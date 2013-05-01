batpack-examples
================
Audio/*
Contains an audio wrapper system for Unity that uses a singleton pattern an a limited AudioSource pool.  This removes the need for GameObjects to have and manage an AudioSource component.  It also contains a MusicManager that creates and manages a persistent object for playing music, and continues playing a track when transitioning between scenes if both scenes use the same AudioClip for music.

Mesh Creation/LevelChunk.cs
Creates 2D meshes in 3D space from a series of given nodes and a material.  Optionally, a collision mesh can also be created by building a second, extruded mesh with outward-facing normals.  When selected, TrackCreator is used to then outline the object with the given colour.

Mesh Creation/MeshExtrusion.cs
Given an existing mesh, assumed to be flat, this will create and return a new mesh extruded by half of the given depth in both directions along the Z axis.  The returned mesh can be used for collision.  This is necessary since only SphereColliders collide with the edge of a triangle, whereas BoxColliders only collider with the face.

Mesh Creation/PlaneCreator.cs
This is used to create a GameObject with a mesh that is a 4 vert, 2 tri plane facing along the z-negative axis (generally toward the camera).  This is used instead of Unity's built-in Plane, which is 200 tris.

Mesh Creation/TrackCreator.cs
This is used to create a 2D track that follows a series of nodes.  First, quads are formed with a given thickness and positioned to connect each node pair of the track together.  Then, the gaps where two track quads connect at a non-180 degree angle is filled in with another triangle.  Optionally, verts can be assigned such that the tiling of the given texture is looped and remains consistent.

AnimationQueue.cs
Allows the the creation of a queue of animations, each step containing up to three delegate functions for start, update and skip.  By updating the AnimationQueue object, the update method assigned by the current animation will be called and is responsible for determining when to move to the next step of animation.  LevelResultsAnimated.cs provides an example of usage.

GameState.cs
Used to manage the state of the game and issue events when the state changes.  The pause state can optionlly allow for setting the Physics TimeStep to 0.  This object also listens for the Application messages OnApplicationFocus and OnApplicationPause.