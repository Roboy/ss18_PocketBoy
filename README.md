# ss18_PocketBoy

PocketBoy is an AR learning app for children in the field of robotics. It is available for Android for smartphones which support [ARCore](https://developers.google.com/ar/discover/supported-devices).

# Installation Guide:

1) clone reposotory: 

```bash
git clone https://github.com/Roboy/ss18_PocketBoy.git
```

2) Install [Unity 2018.2.12f1](https://unity3d.com/de/get-unity/download/archive)

3) Install Android SDK and setup the path in Unity to the SDK, click [here](https://docs.unity3d.com/Manual/android-sdksetup.html) for further instructions

4) Open the project in Unity

# PocketBoy uses Google Cloud TTS, if you have a valid API key, if not proceed with step 5:

4a) Create a TTS configuration file in the path shown in the picture to avoid publishing your key, as the contents of the folder are ignored by a local .gitignore

![create-tts](link1)
![tts-path](link2)

4b) Setup the TTS configuration file

![setup-tts](link3)

4c) Drag the TTS configuration file to the Roboy prefab

![roboy-path](link4)
![roboy-tts](link5)

5) Switch to Android platform in the build settings and press on build

![build](link6)

6) If you have build errors due to some error from Gradle saying that the NDK bundle is missing, switch to internal

![build-internal](link7)