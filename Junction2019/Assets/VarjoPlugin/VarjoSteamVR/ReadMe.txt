Varjo SteamVR is built on Valve's SteamVR Unity plugin:
https://github.com/ValveSoftware/steamvr_unity_plugin

You can use all tracked device and input functionality from Varjo SteamVR the same way they work in normal SteamVR.
Everything else is handled by Varjo SDK. You can treat Varjo SteamVR as trimmed down version of normal SteamVR.

Varjo SteamVR is wrapped inside Varjo namespace and it is safe to import into Unity project that has SteamVR plugins already.
This can cause issues with prefabs, so make sure all Varjo SteamVR prefabs still refer to Varjo SteamVR scripts.

When using Varjo SteamVR with normal SteamVR in same project disable 'Virtual Reality Supported' from PlayerSettings and disable 'Automatically Enable VR' from SteamVR preferences.
Also disable or delete all SteamVR related prefabs from the scene you are using.
If you fail to do these, SteamVR compositor will open and cause issues with Varjo compositor.
