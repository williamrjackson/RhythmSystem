# RhythmSystem

Basic notification-based rhythm system for Unity.

Add a guide loop to the RhythmTracker prefab's AudioSource and specify how many subdivisions you want. For each subdivision, a `UnityAction` is raised, as well as one for every other, fourth, eighth, etc.

To achieve tempo changes, use a 100bpm loop, and manipulate it's speed value.  
1.00 = 100bpm  
1.20 = 120bpm  
.90 = 90bpm, and so on.