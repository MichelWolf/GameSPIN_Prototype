using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
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
    public AudioClip landingSound;
    public AudioClip attack;
    public AudioSource audioSource;
    public AudioClip dashSound;
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
	public GameObject[] firePoint;
    public GameObject[] vfx;
	
	public GameObject uiForPlayer;
	
	private GameObject effectToSpawn;
	private GameObject effectToSpawnStart;
	private GameObject effectToSpawnImplosion;
	private InputManagerIF inputManager;
	private int hitStatus;
	private int hitBy2ndPlayer;
	private Color playercolor2nd;
	private bool cooldown=false;
	public Animator animDummy;
	
    public GameObject teleportParticleStart;
    public GameObject teleportParticleEnd;
    public float dashCooldown;
    private float timeSinceDashed;

    private float moveV, moveH;
    PostProcessingProfile ppp;
    ChromaticAberrationModel.Settings chromAbbSettings;

    internal float startedMoving;
    public float accelerationTime;
    public float accelerationMultiplier = 0;
    internal float lastInputX = 0;
    internal float lastInputZ = 0;

    void Start()
	{
        if (!mouse){
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
		effectToSpawn = vfx[0];
		effectToSpawnStart = vfx[1];
		effectToSpawnImplosion = vfx[2];
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
        timeSinceDashed = dashCooldown;
        ppp = GetComponentInChildren<PostProcessingBehaviour>().profile;
        //animDummy.SetFloat("AnimSpeed", 1f);
        //anim.SetFloat("AnimSpeed", 1f);
        //SendHPToUIMan();
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
        verticalVelocity = Mathf.Clamp(verticalVelocity, -5, 50);
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
			ColliderPartGolem colPartHit = hit.collider.gameObject.GetComponent<ColliderPartGolem>();
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

				EnemyPartHit enemyPart = colPartHit.collisionObject.GetComponent<EnemyPartHit>();

				if(enemyPart.hitByPlayers == 0 || enemyPart.hitByPlayers == hitBy2ndPlayer ){
				   enemyPart.hitByPlayers += hitStatus;
				}
				
				if(enemyPart.hitByPlayers == 3){
					switchColor(colPartHit.collisionObject, Color.blue);
				}else{
					switchColor(colPartHit.collisionObject, playercolor);
				}
            }
        }
        else
        {			
        }
        #endregion Gegner angucken

        moveH = inputManager.MainHorizontal();
        moveV = inputManager.MainVertical();

        anim.SetFloat("VelX", moveH, 1f, Time.deltaTime * 10f);
        anim.SetFloat("VelY", moveV, 1f, Time.deltaTime * 10f);

        animDummy.SetFloat("VelX", moveH, 1f, Time.deltaTime * 10f);
        animDummy.SetFloat("VelY", moveV, 1f, Time.deltaTime * 10f);
      


        if (inputManager.AttackButtonRight()){
				anim.SetTrigger("AttackRight");
				animDummy.SetTrigger("AttackRight");
        }
        if (inputManager.AttackButtonLeft())
        {
            anim.SetTrigger("AttackLeft");
            animDummy.SetTrigger("AttackLeft");
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
                audioSource.PlayOneShot(landingSound, 0.3f);
			}

			falling = false;
		}


        if (inputManager.DashButtonDown() && timeSinceDashed > dashCooldown)
        {
            Instantiate(teleportParticleStart, this.gameObject.transform.GetChild(0).transform.position, this.gameObject.transform.GetChild(0).transform.rotation);
            audioSource.PlayOneShot(dashSound, 0.15f);
            charContr.Move(moveDirection * Time.deltaTime * 50f);
            Instantiate(teleportParticleEnd, this.gameObject.transform.GetChild(0).transform.position, this.gameObject.transform.GetChild(0).transform.rotation);
            timeSinceDashed = 0f;
            chromAbbSettings.intensity = 5;
            ppp.chromaticAberration.settings = chromAbbSettings;
        }
        else
        {
            charContr.Move(moveDirection * Time.deltaTime);
        }
        
        if(timeSinceDashed <= dashCooldown)
        {
            chromAbbSettings.intensity = 5 - 5*(timeSinceDashed/dashCooldown);
            ppp.chromaticAberration.settings = chromAbbSettings;
            timeSinceDashed += Time.deltaTime;
            if (mouse)
            {
                ui_man.UpdateTPCD(0, timeSinceDashed / dashCooldown);
            }
            else
            {
                ui_man.UpdateTPCD(1, timeSinceDashed / dashCooldown);
            }
        }
			
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
                    animDummy.SetTrigger("Run");
                    anim.SetTrigger("Run");
                }
			}
			return;
		}

		if (inputManager.CrouchButton())
		{
			if (walkingMode == WalkingMode.crouching)
			{
				//Testen ob Spieler aufstehen kann
				if (true)
				{
					SwitchWalkingMode(WalkingMode.walking);
                  //  animDummy.SetTrigger("Walk");
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
            if(newMode == WalkingMode.walking)
            {
                animDummy.SetTrigger("Walk");
                anim.SetTrigger("Walk");
            }
            else
            {
                animDummy.SetTrigger("Run");
                anim.SetTrigger("Run");
            }
        }
        else if (newMode == WalkingMode.crouching)
        {
            charContr.height = crouchHeight;
            charContr.center = new Vector3(0f, crouchHeight / 2, 0f);
            animDummy.SetTrigger("Crouch");
            anim.SetTrigger("Crouch");
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
		uiForPlayer.GetComponent<UiScript>().activateDamageEffect();
        //Cooldown zum regenerieren wird gesetzt
        healthRegenCooldown = timeForHealthRegen;
		//wenn der Spieler kein Leben übrig hat
		if (currentHealth <= 0 && oldHealth > 0)
		{
			//StartCoroutine (Respawn ());
		    gameObject.tag = "Untagged";
			ui_man.informPlayerDeath();
			this.enabled = false;
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
        float inputX = inputManager.MainHorizontal();
        float inputZ = inputManager.MainVertical();

        //Beschleunigen
        if ((lastInputX == 0 && lastInputZ == 0) && (inputX != 0 || inputZ != 0))
        {
            startedMoving = Time.time;
        }
        accelerationMultiplier = Mathf.Clamp01((Time.time - startedMoving) / accelerationTime);
        lastInputX = inputX;
        lastInputZ = inputZ;
        moveDirection = new Vector3(inputX, 0, inputZ);

        moveDirection = transform.TransformDirection(moveDirection);

		switch (walkingMode)
		{
		case WalkingMode.crouching:
			moveDirection *= crouchSpeed;
            moveDirection *= accelerationMultiplier;

            break;
		case WalkingMode.running:
			moveDirection *= runSpeed;
                moveDirection *= accelerationMultiplier;
                break;
		case WalkingMode.walking:
			moveDirection *= walkSpeed;
                moveDirection *= accelerationMultiplier;
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
	
	void SpawnVFX(int i){
		//if(!cooldown){
		//	cooldown=true;
		GameObject vfx;
		if(firePoint[i] != null){
			vfx = Instantiate(effectToSpawn, firePoint[i].transform.position, camera.transform.rotation);
			Instantiate(effectToSpawnStart, firePoint[i].transform.position, camera.transform.rotation);
			vfx.GetComponent<PlayerProjectileMove>().setUiScript(uiForPlayer.GetComponent<UiScript>());
            audioSource.PlayOneShot(attack, 0.15f);
        } else {
			Debug.Log("No Fire Point");
		}
			//StartCoroutine(coolDown(.5f));
		//}
		
	}
	
	void SpawnVFXImplosion(int i){
		//if(!cooldown){
		//	cooldown=true;
		GameObject vfx;
		if(firePoint[i] != null){			
			Instantiate(effectToSpawnImplosion, firePoint[i].transform.position, camera.transform.rotation);
		} else {
			Debug.Log("No Fire Point");
		}
			//StartCoroutine(coolDown(.5f));
		//}
		
	}
	
	IEnumerator coolDown(float sec)
    {
        yield return new WaitForSeconds(sec);
		cooldown = false;
    }
}