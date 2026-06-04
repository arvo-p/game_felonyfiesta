using System.Windows.Input;

public class Player : Entity{
	List<Weapon> weapons = new List<Weapon>();
	public Inventory inventory;
	
	short countWeapon = 0;
	short _idxSelectedWeapon = 0;
	public short idxSelectedWeapon{get=>_idxSelectedWeapon;set=>_idxSelectedWeapon=value<countWeapon?value:(short)(countWeapon-1);}
	public Weapon? selectedWeapon{get => countWeapon!=0?weapons[_idxSelectedWeapon]:null;}

	Sprite walk = null!;
	Sprite meleethrow = null!;  
	Sprite stand = null!;
	Sprite death = null!;
	Sprite fire = null!;
	
	Autoaim autoaim;
	Keyboard mykeyboard;
	public bool isKeyboardOn = true;
	
	
	public int score = 0;
	public Player(Crosshair crosshair){
		this.env = Game.env;

		mykeyboard = new Keyboard(new Keys[]{Keys.Z, Keys.Q, Keys.S, Keys.D, Keys.E, Keys.Space, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.R, Keys.Enter, Keys.Tab});
		autoaim = new Autoaim(this, crosshair);

		LoadSprites();
		_sprite = stand;
		_spriteNext = stand;

		setHealth(100);
	
		r.Location = new Point(0, 0);
		r.Size = new Size(60, 60);
		mass = 90;
		SetCollisionCircles();

		weapons.Add((Weapon)env.weaponFootprints.list[(int)Weapon.WeaponType.Pistol].Clone(this));
		
		countWeapon = (short)weapons.Count; 
		idxSelectedWeapon = 3;
		
		inventory = new Inventory(this);
	}

	void LoadSprites(){
		walk = new Sprite(Resources.Player._walk);
		meleethrow = new Sprite(Resources.Player._meleethrow,0,4);
		stand = new Sprite(Resources.Player._stand);
		death = new Sprite(Resources.Player._death,-1,4);
		fire = new Sprite(Resources.Player._fire,0,4);
	}

	bool lastGunHitSuccessful = false;
	public void StartAttack(){
 		Object? victim;

		isAttacking = true;
		if(countWeapon > 0 && selectedWeapon != null){
			/*  Starts weapon animation and matches
			 *  the character's speed (slothFactor) to
			 *  the weapon speed.
			 *  That way both animation are in sync.
			 */
			selectedWeapon.Shoot();
			_sprite = fire;
		    _sprite.MatchSlothFactor(selectedWeapon.sprite); 
			
			if(selectedWeapon.type == Weapon.Type.Gun){
				victim = HitscanCheck(this.center, 400);   
				if(lastGunHitSuccessful = (victim != null)) victim!.IsHit(selectedWeapon.damage, rotation, this);
			}else{
				victim = env.IsObjectColliding(this, 13f, null!, 0);
				if(victim != null) victim!.IsHit(selectedWeapon.damage, rotation, this);
			}
            return;
		}

	   	sprite.Trigger();
	   	_sprite = meleethrow;
		victim = env.IsObjectColliding(this, 13f, null!, 0);
		if(victim != null) victim!.IsHit(15, rotation, this);

		return;
	}

	private void HandleInput(){
		mykeyboard.ReadKeys();

		if (Game.isTurnBased){
			HandleTurnBasedInput();
			return;
		}
		
		if(mykeyboard.GetKeyOnce(Keys.R))
			if(selectedWeapon != null) selectedWeapon.Reload();

		if(autoaim.isAutoaiming){
			if(mykeyboard.GetKeyOnce(Keys.Q)) autoaim.SelectNext(-1);
			if(mykeyboard.GetKeyOnce(Keys.D)) autoaim.SelectNext(1);
		}else{
			if(mykeyboard.GetKey(Keys.Q)) rotation -= 7;
			if(mykeyboard.GetKey(Keys.D)) rotation += 7;
		}

		if(mykeyboard.GetKey(Keys.Z)){
			lastGunHitSuccessful = false;
			autoaim.Set(false);
			speed += 3;
		}

		if(mykeyboard.GetKey(Keys.S)){
			lastGunHitSuccessful = false;
			autoaim.Set(false);
			speed -= 3;
		}

		if(isAttacking){
			if(selectedWeapon != null && selectedWeapon.sprite.isAnimationFinished){
				selectedWeapon.EndShoot();
				isAttacking = false;
			}
			return;
		}
		
		if(mykeyboard.GetKeyOnce(Keys.D1)) idxSelectedWeapon = 0;
		if(mykeyboard.GetKeyOnce(Keys.D2)) idxSelectedWeapon = 1;
		if(mykeyboard.GetKeyOnce(Keys.D3)) idxSelectedWeapon = 2;
		if(mykeyboard.GetKeyOnce(Keys.D4)) idxSelectedWeapon = 3;

		if(mykeyboard.GetKey(Keys.Space)){
			if(speed < 0.3){
				if(!(autoaim.isAutoaiming == true && autoaim.crosshair.isLockedOnTarget == false)){ //test
					if(lastGunHitSuccessful == false && selectedWeapon != null && selectedWeapon.type != Weapon.Type.Melee){
						autoaim.UpdateList();
						if(!autoaim.SelectNext(0))
						StartAttack();
					}
					StartAttack();
					speed = 0;
					return;
				}
			} else StartAttack();
		}

		if(mykeyboard.GetKeyOnce(Keys.E)) if(ActionKey()) return;
	
		speed = Math.Clamp(speed, -15, 15);
	}

	private void HandleTurnBasedInput(){
		var tm = Game.turnManager;
 		if(tm.currentActivePlayer != this || tm.currentState != TurnManager.TurnState.PlayerTurn) return; //not player's turn

		if(isAttacking){
			if(selectedWeapon != null && selectedWeapon.sprite.isAnimationFinished){
				selectedWeapon.EndShoot();
				isAttacking = false;
			}
			return;
		}

		if(tm.playerState == TurnManager.PlayerTurnState.Selection){
			// Rotate player (and arrow follows)
			if(mykeyboard.GetKey(Keys.Q)) this.rotation -= 5;
			if(mykeyboard.GetKey(Keys.D)) this.rotation += 5;

			// Amplitude
			if(mykeyboard.GetKey(Keys.Z)) tm.movementAmplitude = Math.Min(Math.Min(50, tm.movementAmplitude + 1), tm.playerAP/2);
			if(mykeyboard.GetKey(Keys.S)) tm.movementAmplitude = Math.Max(0, tm.movementAmplitude - 1);

			// Confirm Move
			if(mykeyboard.GetKeyOnce(Keys.Enter) && tm.movementAmplitude > 0){
				int cost = (int)(tm.movementAmplitude * 2);
				if(tm.playerAP >= cost){
					this.speed = tm.movementAmplitude/3; 
					tm.playerAP -= cost;
				}else tm.playerAP = 0;
				tm.playerState = TurnManager.PlayerTurnState.Moving;
				tm.movementAmplitude = 0;
			}

			// Enter Aiming Mode
			if(mykeyboard.GetKeyOnce(Keys.Space)){
				autoaim.UpdateList();
				if(autoaim.SelectNext(0)) tm.playerState = TurnManager.PlayerTurnState.Aiming;
			}
		}
		else if(tm.playerState == TurnManager.PlayerTurnState.Aiming){
			// Cycle targets
			if(mykeyboard.GetKeyOnce(Keys.Q)) autoaim.SelectNext(-1);
			if(mykeyboard.GetKeyOnce(Keys.D)) autoaim.SelectNext(1);

			// Back to movement mode
			if(mykeyboard.GetKeyOnce(Keys.Space)){
				autoaim.Set(false);
				tm.playerState = TurnManager.PlayerTurnState.Selection;
			}

			// Confirm Shot
			int cost = 20;
			if(mykeyboard.GetKeyOnce(Keys.Enter)){
				if(tm.playerAP > 0){
					StartAttack();
					tm.playerAP -= cost;
					tm.playerState = TurnManager.PlayerTurnState.Aiming;
				}else{
					tm.playerState = TurnManager.PlayerTurnState.Selection;
					autoaim.Set(false);
				}
			}
		}

		// Free actions (Weapon switching)
		if(mykeyboard.GetKeyOnce(Keys.D1)) idxSelectedWeapon = 0;
		if(mykeyboard.GetKeyOnce(Keys.D2)) idxSelectedWeapon = 1;
		if(mykeyboard.GetKeyOnce(Keys.D3)) idxSelectedWeapon = 2;
		if(mykeyboard.GetKeyOnce(Keys.D4)) idxSelectedWeapon = 3;

		// End Turn manually
		if(mykeyboard.GetKeyUp(Keys.Tab)){
            tm.playerAP = 0;
			tm.playerState = TurnManager.PlayerTurnState.Moving;
		}
	}

	private bool ActionKey(){
		Object? collided = env.IsObjectColliding(this, 10f, null!, 0);
		if(collided == null) return false;
		if(collided is Vehicle vehicle){
			inside = vehicle;
			vehicle.MountPassenger(this,mykeyboard);
			isKeyboardOn = false;
			env.All.Remove(this);
			return true;
		}
		return false;
	}

	private Sprite UpdateSprite(){
		if(isDead) return death;
		if(isAttacking && Math.Abs(speed) < 0.3) return fire; 
		if(Math.Abs(speed) > 0.3) return walk;
		return stand;
	}

	public override void Die(){
		base.Die();
		isAttacking = false;
	}

	public override void Update(){
		if(isDead){
			if(doUpdateSprite){
				doUpdateSprite = false;
				_sprite = UpdateSprite();
				_sprite.Trigger();
			}
			return;
		}

		if(autoaim.isAutoaiming == true){
			if(autoaim.crosshair.isOn) autoaim.crosshair.Update();
			float diff = Tools.GetAngleDifference(this.rotation, autoaim.rotation);
			float lerpFactor = 0.1f;
			this.rotation += diff*lerpFactor;
		}

		if(isKeyboardOn == true) HandleInput();
		_sprite = UpdateSprite();
	}

	public override void IsHit(float damage, float rotation){
		float radians = (float)(Math.PI/180)*rotation;

		if(isDead) return;
		
		health += -(int)damage;
		velRepulsion = new PointF((float)Math.Cos(rotation)*2, (float)Math.Sin(rotation)*2);

		Blood.SprayBlood(r.Location, new PointF((float)Math.Cos(radians),(float)Math.Sin(radians)));
	}

	public void TakeItem(ItemDrop item){
		item.TakeMe(this);
		env.All.Remove(item); 
	}

	public void AddWeapon(Weapon.WeaponType type){
		int index = (int)type;
		if (index < 0 || index >= env.weaponFootprints.list.Count) return;
		
		string[]? targetIcon = env.weaponFootprints.list[index].icon.frames_src;
		if (targetIcon != null && weapons.Any(w => w.icon.frames_src != null && w.icon.frames_src.SequenceEqual(targetIcon))) {
			// Already has it. Maybe give some ammo instead? 
			// For now, let's just return.
			return;
		}
		
		weapons.Add((Weapon)env.weaponFootprints.list[index].Clone(this));
		countWeapon = (short)weapons.Count;
	}
}
