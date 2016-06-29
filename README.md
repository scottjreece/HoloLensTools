# HoloLensTools

HoloTestCamera.cs
--------------
This script allows you to run your Unity application directly in Unity, instead of deploying to the HoloLens emulator or to an actual HoloLens.

**Quickstart**
 - 1. Import the script into your unity environment
 - 2. Add it as a component of the main camera
 - 3. Hit the play button
 - 4. Use the forward and side-to-side axis (usualy with the WASD keys) to move around, and use your mouse to look

**Notes**
Clicking will generate an OnSelect event on the object the camera was pointed at, much like doing an air tap. This behavior can be replaced by setting the ObjectClickedHandler to do whatever you wish.


HoloTestingObject.cs
--------------
This script allows you to create objects that will only be around while running in unity. It's good for making fake floors, walls, etc. that won't show up when there are actual surfaces of a room when run on an actual HoloLens.

**Quickstart**
 - 1. Import the script into your unity environment
 - 2. Create a plane where you want the virtual floor to be
 - 3. Add this script as a component of the new fake floor
 - 4. Hit the play button
 - 5. The floor will be there for testing in Unity, but not when run on the actual HoloLens.

**Notes**
The objects that this script is attached to will be destroyed, not just hidden.

