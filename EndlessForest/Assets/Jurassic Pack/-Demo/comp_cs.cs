﻿using UnityEngine;
using System.Collections;


public class comp_cs : MonoBehaviour {
	
	Transform Spine0,Spine1,Spine2,Spine3,Spine4,Spine5,Neck0,Neck1,Neck2,Neck3,Head,Jaw,
	Tail0,Tail1,Tail2,Tail3,Tail4,Tail5,Tail6,Tail7,Tail8,Arm1,Arm2;
	float turn,pitch,pitch2,open,balance,temp,velocity,jumpforce,animcount, Scale = 0.0F;
	bool reset,soundplayed,isdead =false;
	int lodselect=0, skinselect =0, eyeselect=0;
	string infos;
	Animator anim;
	AudioSource source;
	LODGroup lods;
	SkinnedMeshRenderer[] rend;
	public Texture[] skin,eyes;
	public AudioClip Smallstep,Comp_Roar1,Comp_Roar2,Comp_Call1,Comp_Call2,Comp_Jump,Bite;


	int AIState = 0;
	int currentWaypointID = 0;
	Vector3 waypointPosition = new Vector3();
	float sleepUntil = 0;

	void Awake ()
	{
		Tail0 = this.transform.Find ("Comp/root/pelvis/tail0");
		Tail1 = this.transform.Find ("Comp/root/pelvis/tail0/tail1");
		Tail2 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2");
		Tail3 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3");
		Tail4 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3/tail4");
		Tail5 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5");
		Tail6 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6");
		Tail7 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6/tail7");
		Tail8 = this.transform.Find ("Comp/root/pelvis/tail0/tail1/tail2/tail3/tail4/tail5/tail6/tail7/tail8");
		Spine0 = this.transform.Find ("Comp/root/spine0");
		Spine1 = this.transform.Find ("Comp/root/spine0/spine1");
		Spine2 = this.transform.Find ("Comp/root/spine0/spine1/spine2");
		Spine3 = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3");
		Spine4 = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4");
		Spine5 = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5");
		Arm1 = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/left arm0");
		Arm2 = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/right arm0");
		Neck0  = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0");
		Neck1  = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0/neck1");
		Neck2  = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0/neck1/neck2");
		Neck3  = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0/neck1/neck2/neck3");
		Head   = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0/neck1/neck2/neck3/head");
		Jaw    = this.transform.Find ("Comp/root/spine0/spine1/spine2/spine3/spine4/spine5/neck0/neck1/neck2/neck3/head/jaw0");
	
		source = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		lods = GetComponent<LODGroup>();
		rend = GetComponentsInChildren <SkinnedMeshRenderer>();
	}
	
	/*
	void OnGUI ()
	{
		//Skin selection
		if (GUI.Button (new Rect (5,40,40,20), "Skin"))
		{
			if(skinselect!=2) skinselect ++; else skinselect =0;
			rend[0].material.mainTexture = skin[skinselect];
			rend[1].material.mainTexture = skin[skinselect];
			rend[2].material.mainTexture = skin[skinselect];
		}
		
		//Eyes skin selection
		if (GUI.Button (new Rect (50,40,40,20), "Eyes"))
		{
			if(eyeselect!=15) eyeselect ++; else eyeselect =0;
			rend[0].materials[1].mainTexture = eyes[eyeselect];
			rend[1].materials[1].mainTexture = eyes[eyeselect];
			rend[2].materials[1].mainTexture = eyes[eyeselect];
		}
		
		//Get mesh infos
		if(rend[0].isVisible) infos = rend[0].sharedMesh.triangles.Length/3+" triangles";
		else if (rend[1].isVisible) infos = rend[1].sharedMesh.triangles.Length/3+" triangles";
		else if (rend[2].isVisible) infos = rend[2].sharedMesh.triangles.Length/3+" triangles";
		
		//Lod mesh selection
		switch (lodselect)
		{
		case 0:if (GUI.Button (new Rect (5,100,190,30), "LOD_Auto -> " + infos)) { lods.ForceLOD(0); lodselect=1; } break;
		case 1:if (GUI.Button (new Rect (5,100,190,30), "LOD_0 -> " + infos)) { lods.ForceLOD(1); lodselect=2; } break;
		case 2:if (GUI.Button (new Rect (5,100,190,30), "LOD_1 -> " + infos)) { lods.ForceLOD(2); lodselect=3; } break;
		case 3:if (GUI.Button (new Rect (5,100,190,30), "LOD_2 -> " + infos)) {lods.ForceLOD(-1); lodselect=0; } break;
		}

		GUI.Box (new Rect (0, 170, 200, 380), "Help");
		GUI.Label(new Rect(5,200,Screen.width,Screen.height),"Middle Mouse = Camera/Zoom");
		GUI.Label(new Rect(5,220,Screen.width,Screen.height),"Right Mouse = Spine move");
		GUI.Label(new Rect(5,240,Screen.width,Screen.height),"Left Mouse = Attack");
		GUI.Label(new Rect(5,260,Screen.width,Screen.height),"W,A,S,D = Moves");
		GUI.Label(new Rect(5,280,Screen.width,Screen.height),"LeftShift = Run");
		GUI.Label(new Rect(5,300,Screen.width,Screen.height),"Space = Jump");
		GUI.Label(new Rect(5,320,Screen.width,Screen.height),"E = Growl");
		GUI.Label(new Rect(5,340,Screen.width,Screen.height),"num 1 = IdleA");
		GUI.Label(new Rect(5,360,Screen.width,Screen.height),"num 2 = IdleB");
		GUI.Label(new Rect(5,380,Screen.width,Screen.height),"num 3 = IdleC");
		GUI.Label(new Rect(5,400,Screen.width,Screen.height),"num 4 = Eat");
		GUI.Label(new Rect(5,420,Screen.width,Screen.height),"num 5 = Drink");
		GUI.Label(new Rect(5,440,Screen.width,Screen.height),"num 6 = Sleep");
		GUI.Label(new Rect(5,460,Screen.width,Screen.height),"num 7 = Die");
	}
	*/


	void OnCollisionEnter(Collision collision )
	{
		if(collision.gameObject.name == "Terrain"){
			anim.SetBool("Onground", true);
		} 
		else{
			Debug.Log(collision.gameObject.name);
			if(collision.gameObject.name != "Cube" && AIState == 1){
				Vector2 offset = Random.insideUnitCircle * 10;
				waypointPosition = transform.position - (waypointPosition - transform.position);
			}
		}
	}



	void Update ()
	{
		//***************************************************************************************
		//Moves animation controller
		if( AIState == 0){
			if( Random.value < 0.005f && AIState == 0){
				anim.SetBool("Growl", true);
				AIState = 2;
			}
			if( Random.value < 0.005f && AIState == 0){
				anim.SetInteger ("Idle", 1); //Idle 1
				AIState = 2;
			}
			if ( Random.value < 0.005f && AIState == 0){
				anim.SetInteger ("Idle", 2); //Idle 2
				AIState = 2;
			}
			if ( Random.value < 0.005f && AIState == 0){
				anim.SetInteger ("Idle", 3); //Idle 3
				AIState = 2;
			}
			if ( Random.value < 0.005f && AIState == 0){
				anim.SetInteger ("Idle", 4); //Eat
				AIState = 2;
			}
			if ( Random.value < 0.005f && AIState == 0){
				anim.SetInteger ("Idle", 5); //Drink
				AIState = 2;
			}
			if ( Random.value < 0.001f && AIState == 0){
				anim.SetInteger ("Idle", 6); //Sleep
				sleepUntil = Time.time + Random.value * 60 + 10;
				AIState = 3;
			}
			if( Random.value < 0.02f){
				anim.SetInteger("Idle", 0);
				Vector2 offset = Random.insideUnitCircle * 10;
				waypointPosition = transform.position + new Vector3( offset.x, 0.0f, offset.y);
				AIState = 1;
				anim.SetInteger ("State", 1);					
			}
		}
		else if( AIState == 1){
			waypointPosition.y = transform.position.y;
			Vector3 offset = Vector3.ClampMagnitude( waypointPosition - transform.position, 0.1f);

			transform.position = new Vector3( transform.position.x + offset.x, transform.position.y, transform.position.z + offset.z);
			transform.rotation = Quaternion.LookRotation( offset);
			if( (waypointPosition.x - transform.position.x) < 0.1f && (waypointPosition.z - transform.position.z) < 0.1f){
				Debug.Log( System.String.Format( "Waypoint #{0} reached!", currentWaypointID));
				currentWaypointID += 1;
				Debug.Log( "Idle");
				anim.SetInteger ("State", 0);
				AIState = 0;
			}
		}
		else if( AIState == 2){
			anim.SetBool("Growl", false);
			anim.SetInteger( "Idle", 0);
			if( anim.GetCurrentAnimatorStateInfo(0).IsName("Comp|StandA") && !anim.IsInTransition(0)){
				AIState = 0;
			}
		}
		else if( AIState == 3){
			if( Time.time > sleepUntil){
				Debug.Log("Wake up");
				AIState = 2;
			}
		}

		//if (Input.GetKey (KeyCode.E)) anim.SetBool ("Growl", true);
		//else anim.SetBool ("Growl", false);
		
		/*
		//Attack animation controller
		if (Input.GetKey (KeyCode.Mouse0) && Input.GetKey (KeyCode.LeftShift)) anim.SetInteger ("Attack", 2);
		else if (Input.GetKey (KeyCode.Mouse0)) anim.SetInteger ("Attack", 1);
		else anim.SetInteger ("Attack", 0);

		if (Input.GetKey (KeyCode.Space) &&
		    !anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpLoop") &&
		    !anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpLoop") &&
		    !anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack") &&
		    !anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack") &&
		    !anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") &&
		    !anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") &&
		    !anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpDown") &&
		    !anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpDown")) anim.SetBool("Onground", false); //Jump
		else if (Input.GetKey (KeyCode.LeftShift) && Input.GetKey (KeyCode.W)) anim.SetInteger ("State", 4); //Run
		else if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.W)) anim.SetInteger ("State", 3); //Steps
		else if (Input.GetKey (KeyCode.W)) anim.SetInteger ("State", 1); //Walk
		else if (Input.GetKey (KeyCode.S)) anim.SetInteger ("State", -1); //Steps Back
		else if (Input.GetKey (KeyCode.A)) anim.SetInteger ("State", 2); //Strafe+
		else if (Input.GetKey (KeyCode.D))anim.SetInteger ("State", -2); //Strafe-
		else if (Input.GetKey (KeyCode.LeftControl)) anim.SetInteger ("State", -4); //Steps Idle
		else anim.SetInteger ("State", 0); //Idle

		if (Input.GetKey (KeyCode.Alpha1))
			anim.SetInteger ("Idle", 1); //Idle 1
		else if (Input.GetKey (KeyCode.Alpha2))
			anim.SetInteger ("Idle", 2); //Idle 2
		else if (Input.GetKey (KeyCode.Alpha3))
			anim.SetInteger ("Idle", 3); //Idle 3
		else if (Input.GetKey (KeyCode.Alpha4))
			anim.SetInteger ("Idle", 4); //Eat
		else if (Input.GetKey (KeyCode.Alpha5))
			anim.SetInteger ("Idle", 5); //Drink
		else if (Input.GetKey (KeyCode.Alpha6))
			anim.SetInteger ("Idle", 6); //Sleep
		else if (Input.GetKey (KeyCode.Alpha7))
			anim.SetInteger ("Idle", -1); //Kill
		else
			anim.SetInteger ("Idle", 0);



		//Reset spine position
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|SleepLoop") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Die") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandB") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandC") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|EatA") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|EatB") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandEat") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|GroundAttack")
		   ) reset = true; else reset = false;




		//***************************************************************************************
		//Spine control
		if (Input.GetKey (KeyCode.Mouse1) && reset == false)
		{
			turn += Input.GetAxis ("Mouse X") * 0.5F;
			pitch += -Input.GetAxis ("Mouse Y") * 0.5F;
			
			if (pitch < 0.0F)
				pitch2 += -Input.GetAxis ("Mouse Y") * 8.0F;
			else
				pitch2 -= Input.GetAxis ("Mouse Y") * 8.0F;
		}
		else if (turn != 0.0F || pitch != 0.0F || pitch2 != 0.0F)
		{
			if (turn < 0.0F)
			{
				if (turn < -0.25F)
					turn += 0.25F;
				else
					turn = 0.0F; 
			} 
			else if (turn > 0.0F)
			{
				if (turn > 0.25F)
					turn -= 0.25F;
				else
					turn = 0.0F; 
			}
			
			if (pitch < 0.0F)
			{
				if (pitch < -0.25F)
					pitch += 0.25F;
				else {
					pitch = 0.0F;
					reset = false;
				}
			}
			else if (pitch > 0.0F)
			{
				if (pitch > 0.25F)
					pitch -= 0.25F;
				else
				{
					pitch = 0.0F;
					reset = false;
				}
			}
			
			if (pitch2 < 0.0F)
			{
				if (pitch2 < -1.0F)
					pitch2 += 1.0F;
				else
					pitch2 = 0.0F;
			}
		}
		
		//Jaw control
		if (Input.GetKey (KeyCode.E) && (
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe+") ||
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe-")))
			open -= 5.0F; else open += 1.0F;


		//Reset position
		if (balance != 0.0F)
		{
			if (balance < 0.0F)
			{
				if (balance < -0.3F)
					balance += 0.3F;
				else
					balance = 0.0F; 
			}
			else if (balance > 0.0F)
			{
				if (balance > 0.3F)
					balance -= 0.3F;
				else
					balance = 0.0F; 
			}
		}
		*/

		//***************************************************************************************
		//Sound Fx code
		
		//Get current animation point
		animcount = (anim.GetCurrentAnimatorStateInfo (0).normalizedTime) % 1.0F;
		if(anim.GetAnimatorTransitionInfo(0).normalizedTime!=0.0F) animcount=0.0F;
		animcount = Mathf.Round(animcount * 30);


		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Walk") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Walk-"))
		{
			if(soundplayed==false && animcount==11)
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Smallstep,0.05F);
				soundplayed=true;
			}
			if(animcount!=11) soundplayed=false;
		}
		
		
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe-") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe+"))
		{
			if(soundplayed==false && (animcount==11))
			{
				source.pitch=Random.Range(1.1F, 1.25F);
				source.PlayOneShot(Smallstep,0.5F);
				soundplayed=true;
			}
			if(animcount!=11) soundplayed=false;
			
		}

		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandB") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandB"))
		{
			if(soundplayed==false && animcount==2)
			{
				source.pitch=Random.Range(0.9F, 1.1F);
				source.PlayOneShot(Comp_Call1,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=2) soundplayed=false;
		}

		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandC") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandC"))
		{
			if(soundplayed==false && animcount==2)
			{
				source.pitch=Random.Range(0.9F, 1.1F);
				source.PlayOneShot(Comp_Call2,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=2) soundplayed=false;
		}

	
		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandD") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandD"))
		{
			if(soundplayed==false && animcount==2)
			{
				source.pitch=Random.Range(0.9F, 1.1F);
				source.PlayOneShot(Comp_Roar2,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=2) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandE") ||
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandE"))
		{
			if(soundplayed==false &&(animcount==1 || animcount==8 || animcount==16))
			{
				source.pitch=Random.Range(0.9F, 1.1F);
				source.PlayOneShot(Comp_Call1,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=1 && animcount!=8 && animcount!=16) soundplayed=false;
		}

		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|EatA") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|EatA"))
		{

			if(soundplayed==false && animcount==6)
			{
				source.pitch=Random.Range(1.5F, 2.0F);
				source.PlayOneShot(Bite,Random.Range(0.1F, 0.2F));
				source.PlayOneShot(Comp_Jump,Random.Range(0.1F, 0.3F));
				soundplayed=true;
			}
			else if(animcount!=6) soundplayed=false;
		}



		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|AttackA") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|AttackA")||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|AttackB") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|AttackB") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack"))
		{
			if(soundplayed==false &&(animcount==2))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar1,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(soundplayed==false &&(animcount==12))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Bite,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=2 && animcount!=12) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunAttackA") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackA"))
		{
			if(soundplayed==false &&(animcount==2))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar2,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(soundplayed==false &&(animcount==15))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Bite,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=2 && animcount!=15) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunAttackB") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackB"))
		{
			if(soundplayed==false &&(animcount==3))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar1,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(soundplayed==false &&(animcount==15))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Bite,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=3 && animcount!=15) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpAttack") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpAttack"))
		{
			if(soundplayed==false &&(animcount==3))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar2,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(soundplayed==false &&(animcount==20))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Bite,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=3 && animcount!=20) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|GroundAttack") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|GroundAttack"))
		{
			if(soundplayed==false &&(animcount==3))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar1,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(soundplayed==false &&(animcount==5))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Bite,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=3 && animcount!=5) soundplayed=false;

		}

		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Run") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Run") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunGrowl") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunGrowl"))
		{
			
			if(soundplayed==false && animcount==4 && (
				anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunGrowl") ||
				anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunGrowl")))
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Call1,Random.Range(0.75F, 1.00F));
				soundplayed=true;
			} 
	
			if(soundplayed==false && (animcount==10 || animcount==25))
			{
				source.pitch=Random.Range(1.1F, 1.25F);
				source.PlayOneShot(Smallstep,Random.Range(0.4F, 0.6F));
				soundplayed=true;
			}
			else if(animcount!=4 && animcount!=10 && animcount!=25) soundplayed=false;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandJumpUp") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandJumpUp") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpUp") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpUp"))
		{
			if(soundplayed==false && animcount==4)
			{
				source.pitch=Random.Range(0.9F, 1.1F);
				source.PlayOneShot(Comp_Jump,Random.Range(0.5F, 0.75F));
				soundplayed=true;
			}
			else if(animcount!=4 ) soundplayed=false;
			
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpDown") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpDown"))
		{
			if(soundplayed==false && animcount==4)
			{
				source.pitch=Random.Range(0.75F, 1.0F);
				source.PlayOneShot(Smallstep,Random.Range(0.4F, 0.6F));
				soundplayed=true;
			}
			else if(animcount!=4 ) soundplayed=false;
			
		}


		else if (!isdead && ( anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Die") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Die")))
		{
			if(soundplayed==false && animcount==4)
			{
				source.pitch=Random.Range(0.75F, 1.0F);
				source.PlayOneShot(Comp_Roar1,Random.Range(0.5F, 1.0F));
				soundplayed=true;
			}
			if(soundplayed==false && animcount==20)
			{
				source.PlayOneShot(Smallstep,Random.Range(0.5F, 1.0F));
				soundplayed=true;
			}
			else if(animcount!=4 && animcount!=20  ) soundplayed=false;

			if(animcount>20) isdead=true;
		}


		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Rise") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Rise"))
		{
			isdead=false;

			if(soundplayed==false && animcount==1)
			{
				source.pitch=Random.Range(1.0F, 1.25F);
				source.PlayOneShot(Comp_Roar2,Random.Range(0.5F, 1.0F));
				soundplayed=true;
			}

			else if(animcount!=1) soundplayed=false;
		}

		
	}



	//***************************************************************************************
	//Clamp and set bone rotations
	void LateUpdate()
	{
		/*
			balance = Mathf.Clamp (balance, -7.5F, 7.5F);
			
			if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Run") ||
			    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Run") ||
			    anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunGrowl") ||
			    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunGrowl"))
			{
				turn = Mathf.Clamp (turn, -7.5F, 7.5F);
				pitch = Mathf.Clamp (pitch, 0.0F, 9.0F);
				pitch2 = Mathf.Clamp (pitch2, -20.0F, 0.0F);
			}
			else
			{
				turn = Mathf.Clamp (turn, -15.5F, 15.5F);
				pitch = Mathf.Clamp (pitch, 0.0F, 9.0F);
				pitch2 = Mathf.Clamp (pitch2, -40.0F, 0.0F);
			}
			
			open = Mathf.Clamp (open, -40.0F, 0.0F);
			temp = turn - balance;
			temp += turn;
			temp = Mathf.Clamp (temp, -15.5F, 15.5F);
			
			//Spine left/right
			Spine0.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Spine1.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Spine2.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Spine3.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Spine4.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Spine5.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			
			Neck0.transform.localRotation *= Quaternion.AngleAxis (temp * 2, new Vector3 (0, 0, -1));
			Neck1.transform.localRotation *= Quaternion.AngleAxis (temp * 2, new Vector3 (0, 0, -1));
			Neck2.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			Neck3.transform.localRotation *= Quaternion.AngleAxis (temp / 2, new Vector3 (0, 0, -1));
			Head.transform.localRotation *= Quaternion.AngleAxis (temp, new Vector3 (0, 0, -1));
			
			//Turning
			Tail0.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail1.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail2.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail3.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail4.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail5.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail6.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail7.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));
			Tail8.transform.localRotation *= Quaternion.AngleAxis (balance, new Vector3 (0, 0, -1));

			//Jaw
			Jaw.transform.localRotation *= Quaternion.AngleAxis (open, new Vector3 (-1, 0, 0));
			
			//Spine up/down
			Spine0.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Spine1.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Spine2.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Spine3.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Spine4.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Spine5.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Neck0.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Neck1.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Neck2.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Neck3.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			Head.transform.localRotation *= Quaternion.AngleAxis (pitch, new Vector3 (-1, 0, 0));
			
			//Neck up/down
			Spine0.transform.localRotation *= Quaternion.AngleAxis (pitch2, new Vector3 (-1, 0, 0));
			Arm1.transform.localRotation *= Quaternion.AngleAxis (-pitch2, new Vector3 (-1, 0, 0));
			Arm2.transform.localRotation *= Quaternion.AngleAxis (-pitch2, new Vector3 (0, -1, 0));
			Neck0.transform.localRotation *= Quaternion.AngleAxis (pitch2, new Vector3 (-1, 0, 0));
			Neck1.transform.localRotation *= Quaternion.AngleAxis (pitch2, new Vector3 (-1, 0, 0));
			Head.transform.localRotation *= Quaternion.AngleAxis (-pitch2, new Vector3 (-1, 0, 0));
		*/
	}

	
//***************************************************************************************
//Model translations and rotations
void FixedUpdate ()
{
		/*
		//adjust speed to the model's scale
		Scale = this.transform.localScale.x;
		//adjust gravity to the model's scale
		Physics.gravity = new Vector3(0, -Scale*40.0f, 0);


		//Walking
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Walk") ||
		    	 anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Walk"))

		{
			if(Input.GetKey(KeyCode.A))
			{
				transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
				balance += 0.5F;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
				balance -= 0.5F;
			}


			if (velocity < 0.2F)
			{
				velocity = velocity + (Time.deltaTime * 0.5F); //acceleration
			}
			else if (velocity > 0.2F)
			{
				velocity = velocity - (Time.deltaTime * 0.5F); //acceleration
			}
			
			this.transform.Translate (0, 0, velocity*Scale);
		}

			else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Walk-") ||
			    anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Walk-"))
				
			{
				if(Input.GetKey(KeyCode.D))
				{
					transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
					balance += 0.5F;
				}
				else if(Input.GetKey(KeyCode.A))
				{
					this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
					balance -= 0.5F;
				}
				
				
				if (velocity < 0.2F)
				{
					velocity = velocity + (Time.deltaTime * 0.5F); //acceleration
				}
				else if (velocity > 0.2F)
				{
					velocity = velocity - (Time.deltaTime * 0.5F); //acceleration
				}
				
			this.transform.Translate (0, 0, -velocity*Scale);
			}


		//Running
		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Run") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Run") ||
			     anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunGrowl") ||
			     anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunGrowl") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackA") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunAttackA") ||
			     anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackB") ||
			     anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunAttackB") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpDown") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpDown"))
		{
			if(Input.GetKey(KeyCode.A))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
				balance += 0.5F;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
				balance -= 0.5F;
			}
			
			if (velocity < 0.4F)
			{
				velocity = velocity + (Time.deltaTime * 2.5F);
			}
			
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackA") &&
			    anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.8) velocity =0.0F;

				if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunAttackB") && velocity > 0.2F)
					velocity = velocity - (Time.deltaTime * 2.5F);
			
			
			this.transform.Translate (0, 0, velocity*Scale);
		}


		//Strafe-
		else if (anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Strafe-") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe-"))
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (turn, new Vector3 (0, 1, 0));
			}
			
			if (velocity < 0.1F)
			{
				velocity = velocity + (Time.deltaTime * 0.5F); //acceleration
			}
			else if (velocity > 0.1F)
			{
				velocity = velocity - (Time.deltaTime * 0.5F); //acceleration
			}
			
			this.transform.Translate (velocity*Scale,0,0);
		}


		//Strafe+
		else if ( anim.GetNextAnimatorStateInfo (0).IsName ("Comp|Strafe+") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|Strafe+"))
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (turn, new Vector3 (0, 1, 0));
			}
			
			if (velocity < 0.1F)
			{
				velocity = velocity + (Time.deltaTime * 0.5F); //acceleration
			}
			else if (velocity > 0.1F)
			{
				velocity = velocity - (Time.deltaTime * 0.5F); //acceleration
			}
			
			this.transform.Translate (-velocity*Scale,0,0);
		}
		

		//Stand jump
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandJumpUp") ||
			anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandJumpUp"))
		{
			anim.SetBool("Onground", false);
			
			if(anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.5 && jumpforce<0.5F )
				jumpforce = jumpforce + (Time.deltaTime * 10.0F);
			
			this.transform.Translate (0,jumpforce*Scale, 0);
		}


		//Running jump
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpUp") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpUp"))
		{
			anim.SetBool("Onground", false);

			if(Input.GetKey(KeyCode.A))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
				balance += 0.5F;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
				balance -= 0.5F;
			}

			if(anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.5F && jumpforce<0.5F)
			{
				jumpforce = jumpforce + (Time.deltaTime * 10.0F);
			}

			if (velocity < 0.2F)
			{
				velocity = velocity + (Time.deltaTime * 10.0F);
			}

			this.transform.Translate (0,jumpforce*Scale, velocity*Scale);
		}


		//Jump loop
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpLoop") ||
			         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpLoop") ||
			     anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack") ||
			         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|JumpLoopAttack"))
		{

			if(Input.GetKey(KeyCode.A))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
				balance += 0.5F;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
				balance -= 0.5F;
			}

			if (jumpforce > 0.0F)
			{
				jumpforce = jumpforce - (Time.deltaTime * 2.0F);
			}

			if (velocity > 0.0F)
			{
				velocity = velocity - (Time.deltaTime * 0.01F);
			}

			this.transform.Translate (0, jumpforce*Scale, velocity*Scale);
		}


		//Jump landing
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|StandJumpDown") ||
		         anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpDown") ||
		         anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpDown"))
		{

			jumpforce =0.0F;

			if(Input.GetKey(KeyCode.A))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
				balance += 0.5F;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
				balance -= 0.5F;
			}

			if (velocity < 0.2F &&
			    anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|RunJumpDown") ||
			    anim.GetNextAnimatorStateInfo (0).IsName ("Comp|RunJumpDown"))
			{
				velocity = velocity + (Time.deltaTime * 2.5F);
			}
			else velocity =0.0F;

			this.transform.Translate (0, jumpforce*Scale, velocity*Scale);
		}


		//Jump Attack
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|JumpAttack"))
		{
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.4 && anim.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.9)
			{

				if(Input.GetKey(KeyCode.A))
				{
					this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, -1, 0));
					balance += 0.5F;
				}
				else if(Input.GetKey(KeyCode.D))
				{
					this.transform.localRotation *= Quaternion.AngleAxis (2.0F, new Vector3 (0, 1, 0));
					balance -= 0.5F;
				}


				if (velocity < 0.2F)
				{
					velocity = velocity + (Time.deltaTime * 5.0F);
				}
			} else velocity = 0.0F;
			
			this.transform.Translate (0, 0, velocity*Scale);
		}


		//Stop
		else if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Comp|StandA"))
				{
					velocity =0.0F;
					this.transform.Translate (0, 0, velocity*Scale);
				}
	*/

}


	
	
	
	
	
	
	
	
	
	
}



