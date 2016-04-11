Design decisions:
	I haven't actually played the original game, but watching random YouTube videos really impressed me. I decided to try to stick to it as much as I could without hand-building levels.
	The other idea I was toying with was having an inverted cone and making a Tornado game where you pick up larger and larger things, destroying farms. Decided it'd be a bit heavy on assets and maybe offer less finesse in terms of player controls - rolling around larger objects, break-offs, etc.
	

Controls:
Keyboard:
	WASD to move, QE to turn
XBox One Controller:
	based on http://strategywiki.org/wiki/Katamari_Damacy/Controls
	both sticks forward -> 0, 0, 1
	both sticks back -> 0, 0, -1
	both sticks left -> -1, 0, 0
	both sticks right -> 1, 0, 0
	left forward, right back -> 0, 1, 0
	left back, right forward -> 0, -1, 0
Note: I'm not sure how the XBone controller works on platforms other than Windows. I think it might have some differently-named axes based on platform.

General comments:
	I dislike having lots of MonoBehaviours with their own Update methods. Since Unity calls all of the life cycle stuff through Reflection, it kind of freaks me out. I tend to have only a few objects with Update, etc.

Main is the first class I'd look at. Services are initialized and driven there, and models live there. It is accessible as a Singleton, mostly to expose models.
	Main also kicks off the CollectibleObjectSpawner, which creates a bunch of random CollectibleObjects in the scene at the start of the game.
CollectibleObjects are pretty simple - they're just things that the Katamari can roll up. They have mass, volume, some sounds, etc.
Katamari is probably the next class to look at, since it's the heaviest lifter.
	Katamari has a SphereCollider, a mass property, a CameraBoom, and a KatamariInput.
KatamariInput is the interface for user input. It has two implementations: DualThumbstickInput and KeyboardInput.
	The XboxControllerService is driven by Main, and it polls for the presence of an Xbox controller on the system. If one is added/removed, it'll broadcast an EventManager.InputChanged with a new KatamariInput for the Katamari to use.
	Its only method signature is Vector3 Update(Katamari katamari); which Katamari calls on Update()
	the Vector3 returned is consumed in FixedUpdate, ProcessInput, which applies torque and forces to the Katamari's RigidBody based on the Katamari's mass property.
	One important note here is that it's not scaling based on the RigidBody's mass, but the Katamari.mass property. More on that later.
	Another note is that the Katamari has a different scalar for applying forces when airborne/grounded. You could get moving way too fast when airborne before.
		Ground check is just done with a ray cast downward by radius + 0.01f
The MainCamera has a CameraBoom component, driven by the Katamari Update. It has an offset direction (used as a unit vector). The Katamari supplies it with its radius and current rotationY, and it orbits around the Katamari accordingly.
	I did smooth it out a little bit with a Mathf.Lerp on line 22. It would jump back as the Katamari's radius grew, which looked bad.
The Katamari's SphereCollider's radius grows as more volume is added to it. r = (3V/4PI)^1/3
	I'm just using transform.lossyScale, x * y * z to come up with a simple volume for each object.
Both the Katamari and CollectibleObjects keep a model of their masses stored in properties instead of just on RigidBodies.
	For CollectibleObjects, this is useful in case it breaks off the Katamari and needs to be reinstated on its RigidBody.
	For the Katamari, some of its constituent masses are going to be held on irregularly shaped CollectibleObjects, but we still need to know the total mass when applying input forces/torque.
Some objects are irregularly shaped compared to the Katamari (see CollectibleObject.IsIrregular), and cause the Katamari to bounce irregularly when rolled up. This is a feature in the original game I liked.
	When collected, these are added to irregularCollectibles List and treated a bit differently than others. Their colliders are left on but RigidBodies are destroyed. This makes the Katamari bounce differently.
	Once the Katamari is large enough, irregular objects are treated like normal collected objects, and their colliders are turned off. (I didn't get around to the reverse of this if the Katamari shrinks - not a common case)
If the Katamari hits a larger object with enough velocity, CollectibleObjects can break off. Another feature I liked from the original game. The sphere shrinks, mass decreases, etc.
The Katamari has a little ability to cheat and climb objects in certain cases. This was another feature from the original game that I liked.
	This is handled in the lower portion of Katamari.OnCollisionEnter, and applied in ProcessInput
	I'm checking the upper bounds of whatever you're contacting against a function of your radius to where you first contacted the other object.
		The reasoning here is that I want the Katamari to be able to climb from a smaller block up onto a larger block like stairs.
On collection/detach of CollectibleObjects, we send an Event through EventManager. Our GameModel keeps track of these and sends out ObjectivesUpdated and Victory, accordingly.
	The KatamariUIMediator essentially listens for events and drives the KatamariUIView.
	The KatamariUIView is mostly intended to be stupid, so it only knows how to do as it's told. In this case, displaying victory, collection progress, and information about recently collected objects.
		
As far as external dependencies are concerned, I don't recall adding any. I guess it uses a script from StandardAssets along with the RollerBall prefab for the Katamari.