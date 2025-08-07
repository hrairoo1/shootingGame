using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // �ړ��֘A�̐ݒ�
    [Header("Move")]
    public float moveSpeed = 5f; // �ʏ�ړ����x
    public float dashSpeed = 10f; // �_�b�V�����̑��x
    private float currentSpeed; // ���݂̈ړ����x
    private float targetSpeed;
    public float groundCheckDistance = 1.05f;
    private Vector3 groundNormal = Vector3.up;
    private bool isDush;

    // �W�����v�֘A�̐ݒ�
    [Header("Jump")]
    public float jumpForce = 7f; // �ʏ�W�����v�̗�
    public float boosterForce = 3f; // �u�[�X�g�W�����v�̗�
    public float maxBoosterSpeed = 50f; // �ő�u�[�X�g�W�����v���x
    public float maxFallSpeed = -20f; // �ő嗎�����x
    private float jumpHoldTime = 0f; // �W�����v�{�^���������Ă��鎞��
    public bool isJumping = false;
    private float waitJump = 0;
    private bool isJumpHold = false;
    private float changeJumpTime = 0.15f;

    // �z�o�[���[�h�֘A�̐ݒ�
    [Header("Hover")]
    public float hoverAcceleration = 25f; // �z�o�[���[�h���̉����x
    public float hoverDrag = 0.95f; // �z�o�[���[�h�̋�C��R
    public float hoverFallMultiplier = 0.9f; // �z�o�[���[�h�̗������x�{��

    // �z�o�[����E�_�b�V���֘A
    public float boostDodgeForce = 15f; // �z�o�[����̏���
    public float hoverBoostDodgeMultiplier = 2; // �z�o�[������̔{��
    public float hoverDashSpeed = 40; // �z�o�[�_�b�V���̑��x
    private bool isDodge = false;

    // �G�l���M�[�Ǘ�
    [Header("Enelgy")]
    public float energyMax = 100f; // �ő�G�l���M�[��
    public float energy; // ���݂̃G�l���M�[��
    public float groundRecoveryRate = 1; // ���݂̃G�l���M�[��
    public float energyRecoveryRate = 5f; // �G�l���M�[�񕜑��x
    public float energyBoostConsumption = 20f; // �G�l���M�[����x
    public float energyHoverConsumption = 5f; // �z�o�[���[�h���̃G�l���M�[�����
    public bool energyExhaustion = false;

    private Rigidbody rb; // Rigidbody�R���|�[�l���g
    public bool isGrounded; // �n�ʂɐڂ��Ă��邩�ǂ���
    public bool isHovering;// �z�o�[���[�h���ǂ���
    public bool isBoosting = false;
    private Vector3 dodgeVelocity; // �z�o�[����̑��x�x�N�g��
    private Vector3 hoverDashVelocity; // �z�o�[�_�b�V���̑��x�x�N�g��
    //�L�����N�^�[�����Ǘ�
    public float equipWeight = 1;
    public float thrustPower = 0;
    public float hoverPower = 0;
    public float boostMassFactor;
    public float hoverMassFactor;
    public float massFactor;

    [Header("�n�ʔ���")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("�⓹�z��")]
    public float groundStickForce = 15f;

    // �J������]�֘A
    public Transform cameraTransform; // �J������Transform
    public float mouseSensitivity = 2500f; // �}�E�X���x
    public float cameraRotateSpeed = 0.5f; // �}�E�X���x
    private float rotationX = 0f; // �c��]�p�x
    public Transform cameraTarget; // �v���C���[�̃J������ʒu�i�v���C���[�̓��t�߁j
    public float cameraSmoothSpeed = 0.05f; // �J�����̒Ǐ]���x
    private Vector3 cameraVelocity = Vector3.zero; // �J�����̑��x���Ǘ�
    
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
        //action�C�x���g
        moveAction = controls.Player.Move;
        lookAction = controls.Player.Look;
        controls.Enable();
        jumpAction = controls.Player.Jump; // Jump �A�N�V�����擾
        jumpAction.started += ctx => OnJumpStart();
        jumpAction.canceled += ctx => OnJumpRelease();
        toggleHover = controls.Player.Hover; // Jump �A�N�V�����擾
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
        // �n�ʂ̖@���ɉ����Ĉړ�������␳
        Vector3 moveDirection;
        if (isGrounded)
        {
            moveDirection = Vector3.ProjectOnPlane(inputDirection, groundNormal).normalized;
        }
        else
        {
            moveDirection = inputDirection;
        }

        // �ڕW���x�i�_�b�V���� or �ʏ�ړ����j
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

        // ��Ԃ��đ��x��ύX�i���炩�ɕω��j
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
                // �㏸���͉e�����󂯂Ȃ�
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }
            else
            {
                // ���~���̂� hoverFallMultiplier ��K�p���A�ő嗎�����x�𒴂��Ȃ��悤�ɂ���
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
                jumpHoldTime = 0f; // �������u�ԂɃ��Z�b�g
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
        // 0.15�b�����ŗ������ꍇ�A�ʏ�W�����v
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
            // �ڕW�ʒu�i�v���C���[�̓��t�߁j
            Vector3 targetPosition = cameraTarget.position;

            // `SmoothDamp` ���g���ăJ�����̈ʒu�����炩�ɂ���
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

        //�X���X�g�o��
        float armorThrust = (head != null ? head.thrustPower : 0) + (body != null ? body.thrustPower : 0) + (arm != null ? arm.thrustPower : 0) + (leg != null ? leg.thrustPower : 0) + (backpack != null ? backpack.thrustPower : 0);
        thrustPower = armorThrust;
        boostMassFactor = thrustPower / equipWeight; 
        
        //�z�o�[�o��
        float armorHover = (head != null ? head.hoverPower : 0) + (body != null ? body.hoverPower : 0) + (arm != null ? arm.hoverPower : 0) + (leg != null ? leg.hoverPower : 0) + (backpack != null ? backpack.hoverPower : 0);
        hoverPower = armorHover;
        hoverMassFactor = hoverPower / equipWeight;

        //�G�l���M�[
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
