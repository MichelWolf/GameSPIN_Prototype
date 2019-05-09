using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.PostProcessing;

public class Character : MonoBehaviour
{
	[Header("Bewegung")]
	public float walkSpeed = 5f;
	public float runSpeed = 10f;
	public float crouchSpeed = 3f;
	//Spieler soll nur entweder gehen, schleichen oder rennen können
	public enum WalkingMode { crouching, walking, running };
	public WalkingMode walkingMode = WalkingMode.walking;
	//Spieler soll beim schleichen kleiner sein
	public float standHeight = 2f;
	public float crouchHeight = 1.5f;
	[Header("Gravity")]
	public float verticalVelocity;
	public float gravity = 20.0f;
	public float jumpForce = 10.0f;
	public bool isGrounded = true;
	public bool falling = false;
	private float lastVerticalVelocity = 0;

    //Checking if standing up is possible
    internal Vector3 p1;
    internal Vector3 p2;
    internal int layerMask;

    //Manasystem
    [Header("Manasystem")]
	public float maxMana = 100;
	public float currentMana;
	public float manaRegen = 0.1f;
	public float timeForManaRegen = 5f;
	internal float manaRegenCooldown = 0f;
	public int teleportCost = 10;
	//Lebenssystem
	[Header("Lebenssystem")]
	public float maxHealth = 100;
	public float currentHealth;
	public float healthRegen = 0.1f;
	public float timeForHealthRegen = 10f;
	internal float healthRegenCooldown = 0f;

    internal UI_Manager ui_man;

	//Respawnsystem
	[Header("Respawn")]
	public float respawnTime = 5f;
	public GameObject spawnpoint;

	private CharacterController charContr;

	private Vector3 moveDirection = Vector3.zero;

	Gravity grav;
	//Image fadenkreuzImage;
	camMouseLook mouseLook;

    public Material normalEnemy;
    public Material lockedEnemy;

    Animator anim;
    bool idle = true;
	//PostProcessingProfile ppp;
	//VignetteModel.Settings pppSettings;

	//Control Settings
	[Header("Controller")]
	public bool mouse;	
	public Camera camera;
	public Color playercolor;
	
	[Header("AttackProj")]
	public GameObject firePoint;
	public GameObject vfx;
	
	private GameObject effectToSpawn;
	private InputManagerIF inputManager;
	private int hitStatus;
	private int hitBy2ndPlayer;
	private Color playercolor2nd;
	
	

	void Start()
	{
		if(!mouse){
			inputManager = new InputManagerController();
			hitBy2ndPlayer = 1;
			hitStatus = 2;
			playercolor2nd = Color.red;
		}else{
			inputManager = new InputManagerMouse();
			hitBy2ndPlayer = 2;
			hitStatus = 1;
			playercolor2nd = Color.green;
		}
		 effectToSpawn = vfx;
		Cursor.lockState = CursorLockMode.Locked;
		charContr = GetComponent<CharacterController>();
		currentMana = maxMana;
		currentHealth = maxHealth;
		grav = GetComponent<Gravity> ();
		grav.enabled = false;
		mouseLook = Camera.main.gameObject.GetComponent<camMouseLook> ();

        layerMask = 1 << 8 | 1 << 2;
        layerMask = ~layerMask;
        anim = GetComponentInChildren<Animator>();
        ui_man = FindObjectOfType<UI_Manager>();
    }

	void Update()
	{
		#region Movement
		HandleCurrentObjectLookedAt();
		GetWalkingMode();
		//Raycast von mitte des Spielers bis Hälfte der Spielerhöhe + 0.1f nach unten. wenn etwas getroffen wird ist der Spieler am Boden 
		RaycastHit groundCheck;
		isGrounded = Physics.SphereCast(this.transform.position + charContr.center, charContr.radius, Vector3.down, out groundCheck, charContr.center.y + 0.1f - charContr.radius);

		//wenn der Spieler läuft, Bewegungen mit walkSpeed

		CalculateMoveDirection();

		//Wenn der Spieler die "Jump"-Taste (space) drückt und sich auf dem Boden befindet
		if (isGrounded)
		{
			verticalVelocity = -gravity * Time.deltaTime;
		}
		if (inputManager.JumpButton() && isGrounded)
		{
			verticalVelocity = jumpForce;

		}
		verticalVelocity -= gravity * Time.deltaTime;
		if (!isGrounded)
		{
			falling = true;
		}
		#endregion Movement

		#region Leben und Mana

		RegenerateMana();
		RegenerateHealth();

        #endregion Leben und Mana

        #region Gegner angucken
        RaycastHit hit;
        Vector3 rayOrigin = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        //Raycast in Richtung der Camera, was getroffen wird
        if (Physics.Raycast(rayOrigin, camera.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.GetComponent<ColliderPartGolem>() != null)
            {
                ColliderPartGolem[] e = GameObject.FindObjectsOfType<ColliderPartGolem>();
                foreach(ColliderPartGolem en in e)
                {
					GameObject objectForCollider = en.GetComponent<ColliderPartGolem>().collisionObject;
					int status = objectForCollider.GetComponent<EnemyPartHit> ().hitByPlayers;
                    if (en.gameObject != hit.collider.gameObject)
                    {
						// Nur entfärben wenn auch der zweite Spieler nicht auf den part zeigt
						
						if(status == 3 || status == hitStatus){
							objectForCollider.GetComponent<EnemyPartHit> ().hitByPlayers -= hitStatus;
							if(status == 3){
							objectForCollider.gameObject.GetComponent<Renderer>().material.color =playercolor2nd;
							}
						}else if(status != hitBy2ndPlayer){
							//en.gameObject.GetComponent<MeshRenderer>().material = normalEnemy;	
						//	en.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;	
							switchColor(objectForCollider.gameObject, Color.white);
						}
						//	en.gameObject.GetComponent<MeshRenderer>().material.color = playercolor2nd;
								
                    } 
                }

				if(hit.collider.gameObject.GetComponent<ColliderPartGolem>().collisionObject.GetComponent<EnemyPartHit> ().hitByPlayers == 0 || hit.collider.gameObject.GetComponent<ColliderPartGolem>().collisionObject.GetComponent<EnemyPartHit> ().hitByPlayers == hitBy2ndPlayer ){
				hit.collider.gameObject.GetComponent<ColliderPartGolem>().collisionObject.GetComponent<EnemyPartHit> ().hitByPlayers += hitStatus;
				}
				
				if(hit.collider.GetComponent<ColliderPartGolem>().collisionObject.gameObject.GetComponent<EnemyPartHit> ().hitByPlayers == 3){
					//hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
					switchColor(hit.collider.gameObject.GetComponent<ColliderPartGolem>().collisionObject, Color.blue);
				}else{
					//hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = playercolor;
					switchColor(hit.collider.gameObject.GetComponent<ColliderPartGolem>().collisionObject, playercolor);
				}
				#region angreifen
							
				if(inputManager.AttackButton()){
				//	hit.collider.gameObject.GetComponent<EnemyPartHit>().attack();
				//	firePoint.transform.localRotation = Quaternion.Lerp(firePoint.transform.rotation, Quaternion.LookRotation(camera.transform.forward - firePoint.transform.position),1);
					SpawnVFX();
				}
				#endregion angreifen

            }
        }
        else
        {			
        }
        #endregion Gegner angucken
		
	if(inputManager.AttackButton()){
				//	firePoint.transform.localRotation = Quaternion.Lerp(firePoint.transform.rotation, Quaternion.LookRotation(camera.transform.forward - firePoint.transform.position),1);
					SpawnVFX();
			}

        //Movement ganz am Ende

        if (falling && isGrounded)
		{
			//Debug.Log (lastVerticalVelocity);

			Transform child = transform.GetChild (0);
			if (lastVerticalVelocity < -15f)
			{
				child.position = new Vector3 (child.position.x, Mathf.Max(child.position.y - 0.6f, 1.2f), child.position.z);
				TakeDamage ((int)Mathf.Round (-1 * ((lastVerticalVelocity + 15) * 3)));
			} 
			else
			{
				child.position = new Vector3 (child.position.x, Mathf.Max(child.position.y - 0.2f, 1.2f), child.position.z);
			}

			if(lastVerticalVelocity < -7f)
			{
                //Landen-Sound
			}

			falling = false;
		}

       

        
        if (walkingMode == WalkingMode.walking)
        {
            if (moveDirection.x == 0 && moveDirection.z == 0)
            {
                anim.Play("Idle@Idle");
            }
            else
            {
                anim.Play("Idle@Walking");
            }
        }
        else if (walkingMode == WalkingMode.running)
        {
            anim.Play("Idle@Running");
        }
        else if (walkingMode == WalkingMode.crouching)
        {
            if (moveDirection.x == 0 && moveDirection.z == 0)
            {
                anim.Play("Idle@Crouch Idle");
            }
            else
            {
                anim.Play("Idle@Sneaking Forward");
            }
        }


        charContr.Move(moveDirection * Time.deltaTime);
			
		charContr.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
		
		lastVerticalVelocity = verticalVelocity;
	}
	#region Funktionen
	public void GetWalkingMode()
	{
		if (inputManager.RunButtonDown())
		{
			if ((inputManager.MainJoystick().x == 0 && inputManager.MainJoystick().z == 0) || !isGrounded) 
			{
				return;
			}

			if (walkingMode == WalkingMode.crouching)
			{
				if (!DoesPlayerFit())
				{
					return;
				}
			}

			if (walkingMode == WalkingMode.running)
			{
				SwitchWalkingMode(WalkingMode.walking);
			}
			else
			{
				if (isGrounded) 
				{
					SwitchWalkingMode(WalkingMode.running);
				}
			}
			return;
		}

		if (inputManager.CrouchButton())
		{
			if (walkingMode == WalkingMode.crouching)
			{
				//Testen ob Spieler aufstehen kann
				if (DoesPlayerFit())
				{
					SwitchWalkingMode(WalkingMode.walking);
				}
				else
				{
					return;
				}
			}
			else
			{
				SwitchWalkingMode(WalkingMode.crouching);
			}
			return;
		}
		if ((inputManager.MainJoystick ().x == 0) && (inputManager.MainJoystick ().z == 0) && walkingMode != WalkingMode.crouching)
		{
			SwitchWalkingMode(WalkingMode.walking);
		}
	}

	void SwitchWalkingMode(WalkingMode newMode)
	{
		WalkingMode oldMode = walkingMode;
		walkingMode = newMode;
        // do stuff
        if (oldMode == WalkingMode.crouching && newMode != WalkingMode.crouching)
        {
            charContr.height = standHeight;
            charContr.center = new Vector3(0f, standHeight / 2, 0f);
        }
        else if (newMode == WalkingMode.crouching)
        {
            charContr.height = crouchHeight;
            charContr.center = new Vector3(0f, crouchHeight / 2, 0f);
        }
    }

	public void HandleCurrentObjectLookedAt()
	{
		
	}

	public void TakeDamage(int damage)
	{
		float oldHealth = currentHealth;

		currentHealth -= damage;
        SendHPToUIMan();
        //Cooldown zum regenerieren wird gesetzt
        healthRegenCooldown = timeForHealthRegen;
		//wenn der Spieler kein Leben übrig hat
		if (currentHealth <= 0 && oldHealth > 0)
		{
			StartCoroutine (Respawn ());
		}
	}

	public IEnumerator Respawn()
	{
		//deaktiviere dieses Skript 
		this.enabled = false;
		grav.enabled = true;

		SwitchWalkingMode(WalkingMode.walking);
		//warte bis Respawn-Timer abgelaufen ist
		yield return new WaitForSeconds(respawnTime);
		//aktiviere dieses Skript wieder
		this.enabled = true;
		grav.enabled = false;
		
		//Setzt den Spieler auf die Position des aktuellen Spawnpoints
		if (spawnpoint != null)
		{
			this.transform.position = spawnpoint.transform.position;
		}
		//Setzt Leben und Mana des Spielers wieder auf voll
		currentHealth = maxHealth;
		currentMana = maxMana;
        //uiManager.UpdateHealth(currentHealth);
        //uiManager.UpdateMana(currentMana);
        SendHPToUIMan();
    }

    private void SendHPToUIMan()
    {
        if (mouse)
        {
            ui_man.UpdateHP(0, currentHealth);
        }
        else
        {
            ui_man.UpdateHP(1, currentHealth);
        }
    }

	

	private void RegenerateMana()
	{	
		if (manaRegenCooldown > 0f)
		{
			manaRegenCooldown -= Time.deltaTime;
		}
		if (currentMana < maxMana && manaRegenCooldown <= 0f)
		{
			currentMana += manaRegen;
			//ui_man.UpdateMana(currentMana);
		}
	}

	private void RegenerateHealth()
	{
		if (healthRegenCooldown > 0f)
		{
			healthRegenCooldown -= Time.deltaTime;
		}
		if (currentHealth < maxHealth && healthRegenCooldown <= 0f)
		{
			currentHealth += healthRegen;
            SendHPToUIMan();
        }
	}

	private void CalculateMoveDirection()
	{
		//moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = new Vector3(inputManager.MainHorizontal(), 0, inputManager.MainVertical());

        moveDirection = transform.TransformDirection(moveDirection);

		switch (walkingMode)
		{
		case WalkingMode.crouching:
			moveDirection *= crouchSpeed;
			break;
		case WalkingMode.running:
			moveDirection *= runSpeed;
			break;
		case WalkingMode.walking:
			moveDirection *= walkSpeed;
			break;
		}
	}

	private void MeleeAttack()
	{
		
	}
    #endregion
    internal bool DoesPlayerFit()
    {
            RaycastHit boden;
            if (Physics.SphereCast(charContr.transform.position, charContr.radius, Vector3.down, out boden))
            {
                p1 = boden.point + new Vector3(0.0f, charContr.radius + 0.01f, 0.0f);
                p1.x = charContr.transform.position.x;
                p1.z = charContr.transform.position.z;
                p2 = p1 + Vector3.up * (standHeight / 2);
                if (Physics.CheckCapsule(p1, p2, charContr.radius, layerMask))
                {
                    Debug.Log("Aufstehen passt nicht");
                    return false;
                }
                else
                {
                    Debug.Log("Aufstehen passt");
                    return true;
                }

            }
        
        return true;
    }
	
	void switchColor(GameObject gO, Color clr){					
		foreach(Material mat in gO.GetComponent<Renderer>().materials){
			mat.color = clr;
		}
	}

    void OnApplicationQuit()
	{
		//pppSettings.intensity = 0;
		//ppp.vignette.settings = pppSettings;
	}
	
	void SpawnVFX(){
		GameObject vfx;
		if(firePoint != null){
			vfx = Instantiate(effectToSpawn, firePoint.transform.position, camera.transform.rotation);
		} else {
			Debug.Log("No Fire Point");
		}
	}
}