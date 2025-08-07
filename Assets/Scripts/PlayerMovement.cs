using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 移動関連の設定
    [Header("Move")]
    public float moveSpeed = 5f; // 通常移動速度
    public float dashSpeed = 10f; // ダッシュ時の速度
    private float currentSpeed; // 現在の移動速度
    private float targetSpeed;
    public float groundCheckDistance = 1.05f;
    private Vector3 groundNormal = Vector3.up;
    private bool isDush;

    // ジャンプ関連の設定
    [Header("Jump")]
    public float jumpForce = 7f; // 通常ジャンプの力
    public float boosterForce = 3f; // ブーストジャンプの力
    public float maxBoosterSpeed = 50f; // 最大ブーストジャンプ速度
    public float maxFallSpeed = -20f; // 最大落下速度
    private float jumpHoldTime = 0f; // ジャンプボタンを押している時間
    public bool isJumping = false;
    private float waitJump = 0;
    private bool isJumpHold = false;
    private float changeJumpTime = 0.15f;

    // ホバーモード関連の設定
    [Header("Hover")]
    public float hoverAcceleration = 25f; // ホバーモード中の加速度
    public float hoverDrag = 0.95f; // ホバーモードの空気抵抗
    public float hoverFallMultiplier = 0.9f; // ホバーモードの落下速度倍率

    // ホバー回避・ダッシュ関連
    public float boostDodgeForce = 15f; // ホバー回避の初速
    public float hoverBoostDodgeMultiplier = 2; // ホバー回避時の倍率
    public float hoverDashSpeed = 40; // ホバーダッシュの速度
    private bool isDodge = false;

    // エネルギー管理
    [Header("Enelgy")]
    public float energyMax = 100f; // 最大エネルギー量
    public float energy; // 現在のエネルギー量
    public float groundRecoveryRate = 1; // 現在のエネルギー量
    public float energyRecoveryRate = 5f; // エネルギー回復速度
    public float energyBoostConsumption = 20f; // エネルギー消費速度
    public float energyHoverConsumption = 5f; // ホバーモード時のエネルギー消費量
    public bool energyExhaustion = false;

    private Rigidbody rb; // Rigidbodyコンポーネント
    public bool isGrounded; // 地面に接しているかどうか
    public bool isHovering;// ホバーモードかどうか
    public bool isBoosting = false;
    private Vector3 dodgeVelocity; // ホバー回避の速度ベクトル
    private Vector3 hoverDashVelocity; // ホバーダッシュの速度ベクトル
    //キャラクター装備管理
    public float equipWeight = 1;
    public float thrustPower = 0;
    public float hoverPower = 0;
    public float boostMassFactor;
    public float hoverMassFactor;
    public float massFactor;

    [Header("地面判定")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("坂道吸着")]
    public float groundStickForce = 15f;

    // カメラ回転関連
    public Transform cameraTransform; // カメラのTransform
    public float mouseSensitivity = 2500f; // マウス感度
    public float cameraRotateSpeed = 0.5f; // マウス感度
    private float rotationX = 0f; // 縦回転角度
    public Transform cameraTarget; // プレイヤーのカメラ基準位置（プレイヤーの頭付近）
    public float cameraSmoothSpeed = 0.05f; // カメラの追従速度
    private Vector3 cameraVelocity = Vector3.zero; // カメラの速度を管理
    
    public PlayerWeaponManager playerWeaponManager;
    public PlayerArmorManager playerArmorManager;
    private Collider myCollider;
    [SerializeField] InputSystem_Actions controls;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction toggleHover;
    private InputAction dashAction;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        energy = energyMax;
        cameraTransform.position = cameraTarget.position;
        myCollider = GetComponent<Collider>();
        cameraTransform.SetParent(null);

        controls = new InputSystem_Actions();
        //actionイベント
        moveAction = controls.Player.Move;
        lookAction = controls.Player.Look;
        controls.Enable();
        jumpAction = controls.Player.Jump; // Jump アクション取得
        jumpAction.started += ctx => OnJumpStart();
        jumpAction.canceled += ctx => OnJumpRelease();
        toggleHover = controls.Player.Hover; // Jump アクション取得
        toggleHover.started += ctx => { if (!energyExhaustion || hoverPower > 0) { isHovering = !isHovering; }};
        dashAction = controls.Player.Dash;
        dashAction.started += ctx => { isDush = true; isDodge = true; };
        dashAction.canceled += ctx => { isDush = false; };
    }

    void Update()
    {
        HandleJump();
        HandleHover();
        RecoverEnergy();
        HandleCameraRotation();
        ChangeMass();
        if (energyExhaustion || hoverPower <= 0) { isHovering = false; }
        if (playerArmorManager.boosterEffect.Count != 0)
        {
            foreach(GameObject booster in playerArmorManager.boosterEffect)
            {
                Animator boosterAnim = booster.GetComponent<Animator>();
                if (isHovering && !energyExhaustion) boosterAnim.SetBool("hover", true);
                else boosterAnim.SetBool("hover", false);
                if (isBoosting && !energyExhaustion) boosterAnim.SetBool("boost", true);
                else boosterAnim.SetBool("boost", false);
            }
        }
    }

    void FixedUpdate()
    {
        Move();
        SmoothCameraFollow();
        GroundCheck(); 
        if (isGrounded)
        {
            StickToGround();
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        var axis = moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = transform.right * axis.x + transform.forward * axis.y;
        // 地面の法線に沿って移動方向を補正
        Vector3 moveDirection;
        if (isGrounded)
        {
            moveDirection = Vector3.ProjectOnPlane(inputDirection, groundNormal).normalized;
        }
        else
        {
            moveDirection = inputDirection;
        }

        // 目標速度（ダッシュ時 or 通常移動時）
        if (moveDirection.magnitude > 0)
        {
            if (isDush && isGrounded)
            {
                targetSpeed = dashSpeed;
            }
            else
            {
                targetSpeed = moveSpeed;
            }
        }
        else
        {
            targetSpeed = 0;
        }

        // 補間して速度を変更（滑らかに変化）
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        if (!isHovering)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(moveDirection.x * currentSpeed * massFactor, rb.velocity.y, moveDirection.z * currentSpeed * massFactor);
            }
            if (!isGrounded)
            {

                rb.AddForce(moveDirection *  currentSpeed * massFactor * 2.5f, ForceMode.Acceleration);
            }
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) * hoverDrag;
            if (rb.velocity.y > 0)
            {
                // 上昇時は影響を受けない
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }
            else
            {
                // 下降時のみ hoverFallMultiplier を適用し、最大落下速度を超えないようにする
                rb.velocity = new Vector3(horizontalVelocity.x, Mathf.Max(rb.velocity.y * hoverFallMultiplier, maxFallSpeed), horizontalVelocity.z);
            }
            rb.AddForce(moveDirection * (isDush && !energyExhaustion ? hoverDashSpeed : hoverAcceleration) * hoverMassFactor, ForceMode.Acceleration);
        }
    }
    void OnJumpStart()
    {
        if (thrustPower > 0)
        {
            if ((!isBoosting || isGrounded) && !isHovering)
            {
                isJumpHold = true;
                jumpHoldTime = 0f; // 押した瞬間にリセット
            }
            if ((!isGrounded || isHovering) && !energyExhaustion) isBoosting = true;
        }
        else if(isGrounded)
        {
            StartCoroutine(Jump());
        }
    }
    void OnJumpRelease()
    {
        isJumpHold = false;
        // 0.15秒未満で離した場合、通常ジャンプ
        if (jumpHoldTime < changeJumpTime && isGrounded && energy > 0)
        {
            StartCoroutine(Jump());
        }
            isBoosting = false;
    }


    void HandleJump()
    {
        if (isJumpHold)
        {
            jumpHoldTime += Time.deltaTime;
            if (jumpHoldTime >= changeJumpTime)
            {
                if(!energyExhaustion) isBoosting = true;
                isJumpHold = false;
            }
        }
        if (isBoosting && !energyExhaustion)
        {
            Boost();
        }

    }
    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.05f);
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }
    private void Boost()
    {
        rb.AddForce(Vector3.up * boosterForce * boostMassFactor, ForceMode.Acceleration);
        //rb.velocity = new Vector3(rb.velocity.x, Mathf.Min(rb.velocity.y, maxBoosterSpeed), rb.velocity.z);
        energy -= energyBoostConsumption * Time.deltaTime;
        isBoosting = true;
    }

    void HandleHover()
    {
        if (isHovering && energy > 0)
        {
            energy -= energyHoverConsumption * Time.deltaTime;
        }

        if (isHovering && isDodge && energy > 0)
        {
            var axis = moveAction.ReadValue<Vector2>();
            Vector3 dodgeDirection = transform.right * axis.x + transform.forward * axis.y;
            dodgeDirection.Normalize();

            if (dodgeDirection.magnitude > 0)
            {
                rb.velocity = Vector3.zero;
                hoverDashVelocity = dodgeDirection * boostDodgeForce * hoverBoostDodgeMultiplier * hoverMassFactor;
                rb.velocity += hoverDashVelocity;
                energy -= energyBoostConsumption;
                if (playerArmorManager.boosterEffect.Count != 0)
                {
                    foreach (GameObject booster in playerArmorManager.boosterEffect)
                    {
                        Animator boosterAnim = booster.GetComponent<Animator>();
                        boosterAnim.SetTrigger("dodge");
                    }
                }
            }
        }
        isDodge = false;
    }

    void RecoverEnergy()
    {
        if (!isBoosting && energy < energyMax)
        {
            if (isGrounded && !isHovering && isBoosting != true)
            {
                energy += (energyMax * groundRecoveryRate) * Time.deltaTime;
            }
            else
            {
                if(isBoosting != true)
                energy += energyRecoveryRate * Time.deltaTime;
            }
        }
        if(energy > energyMax)
        {
            energy = energyMax;
        }
        if(energy >= energyMax)
        {
            energyExhaustion = false;
        }
        if(energy < 0)
        {
            energy = 0;
            energyExhaustion = true;
        }
    }
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        var cameraRotate = lookAction.ReadValue<Vector2>() * cameraRotateSpeed;

        rotationX -= cameraRotate.y;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, transform.eulerAngles.y,0f);
        transform.Rotate(Vector3.up * cameraRotate.x);
    }
    void SmoothCameraFollow()
    {
        if (cameraTransform != null && cameraTarget != null)
        {
            // 目標位置（プレイヤーの頭付近）
            Vector3 targetPosition = cameraTarget.position;

            // `SmoothDamp` を使ってカメラの位置を滑らかにする
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, cameraTarget.position, ref cameraVelocity, cameraSmoothSpeed);
        }
    }

    public void ChangeMass()
    {
        Weapon mainR = playerWeaponManager.weaponSlots.ContainsKey("MainR") ? playerWeaponManager.weaponSlots["MainR"].equippedWeapon : null;
        Weapon mainL = playerWeaponManager.weaponSlots.ContainsKey("MainL") ? playerWeaponManager.weaponSlots["MainL"].equippedWeapon : null;
        Weapon subR = playerWeaponManager.weaponSlots.ContainsKey("SubR") ? playerWeaponManager.weaponSlots["SubR"].equippedWeapon : null;
        Weapon subL = playerWeaponManager.weaponSlots.ContainsKey("SubL") ? playerWeaponManager.weaponSlots["SubL"].equippedWeapon : null;
        Weapon shoulderR = playerWeaponManager.weaponSlots.ContainsKey("ShoulderR") ? playerWeaponManager.weaponSlots["ShoulderR"].equippedWeapon : null;
        Weapon shoulderL = playerWeaponManager.weaponSlots.ContainsKey("ShoulderL") ? playerWeaponManager.weaponSlots["ShoulderL"].equippedWeapon : null;
        float weaponWeight = (mainR != null ? mainR.weight : 0) + (mainL != null ? mainL.weight : 0) + (subR != null ? subR.weight : 0) + (subL != null ? subL.weight : 0) + (shoulderR != null ? shoulderR.weight : 0) + (shoulderL != null ? shoulderL.weight : 0);

        Armor head = playerArmorManager.armorSlots.ContainsKey("Head") ? playerArmorManager.armorSlots["Head"].equippedArmor : null;
        Armor body = playerArmorManager.armorSlots.ContainsKey("Body") ? playerArmorManager.armorSlots["Body"].equippedArmor : null;
        Armor arm = playerArmorManager.armorSlots.ContainsKey("Arm") ? playerArmorManager.armorSlots["Arm"].equippedArmor : null;
        Armor leg = playerArmorManager.armorSlots.ContainsKey("Leg") ? playerArmorManager.armorSlots["Leg"].equippedArmor : null;
        Armor backpack = playerArmorManager.armorSlots.ContainsKey("Backpack") ? playerArmorManager.armorSlots["Backpack"].equippedArmor : null;
        float armorWeight = (head != null ? head.weight : 0) + (body != null ? body.weight : 0) + (arm != null ? arm.weight : 0) + (leg != null ? leg.weight : 0) + (backpack != null ? backpack.weight : 0);
        equipWeight = 100 + weaponWeight + armorWeight;

        //スラスト出力
        float armorThrust = (head != null ? head.thrustPower : 0) + (body != null ? body.thrustPower : 0) + (arm != null ? arm.thrustPower : 0) + (leg != null ? leg.thrustPower : 0) + (backpack != null ? backpack.thrustPower : 0);
        thrustPower = armorThrust;
        boostMassFactor = thrustPower / equipWeight; 
        
        //ホバー出力
        float armorHover = (head != null ? head.hoverPower : 0) + (body != null ? body.hoverPower : 0) + (arm != null ? arm.hoverPower : 0) + (leg != null ? leg.hoverPower : 0) + (backpack != null ? backpack.hoverPower : 0);
        hoverPower = armorHover;
        hoverMassFactor = hoverPower / equipWeight;

        //エネルギー
        float enelgyMax = (head != null ? head.energy : 0) + (body != null ? body.energy : 0) + (arm != null ? arm.energy : 0) + (leg != null ? leg.energy : 0) + (backpack != null ? backpack.energy : 0);
        float energyRecoveryRate = (head != null ? head.energyRecoveryRate : 0) + (body != null ? body.energyRecoveryRate : 0) + (arm != null ? arm.energyRecoveryRate : 0) + (leg != null ? leg.energyRecoveryRate : 0) + (backpack != null ? backpack.energyRecoveryRate : 0);
        float energyBoostConsumption = (head != null ? head.energyBoostConsumption : 0) + (body != null ? body.energyBoostConsumption : 0) + (arm != null ? arm.energyBoostConsumption : 0) + (leg != null ? leg.energyBoostConsumption : 0) + (backpack != null ? backpack.energyBoostConsumption : 0);
        float energyHoverConsumption = (head != null ? head.energyHoverConsumption : 0) + (body != null ? body.energyHoverConsumption : 0) + (arm != null ? arm.energyHoverConsumption : 0) + (leg != null ? leg.energyHoverConsumption : 0) + (backpack != null ? backpack.energyHoverConsumption : 0);


        float legPower = leg != null ? leg.weightLimit : 0;
        massFactor = Mathf.Min((legPower + 100) / equipWeight, 1.5f);
    }

    private void GroundCheck()
    {
        Collider[] hits = Physics.OverlapSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);

        isGrounded = false;
        if (isJumping == true || isBoosting) return;
        foreach (var hit in hits)
        {
            if (hit.transform.root != transform.root)
            {
                isGrounded = true;
                break;
            }
        }
    }
    private void StickToGround()
    {
        if(!isHovering)
        rb.AddForce(Vector3.down * groundStickForce, ForceMode.Force);
    }
}
