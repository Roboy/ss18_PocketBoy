# ss18_PocketBoy

PocketBoy is an AR learning app for children in the field of robotics. It is available for Android for smartphones which support [ARCore](https://developers.google.com/ar/discover/supported-devices).

# Installation Guide:

1) Clone repository: 

```bash
git clone https://github.com/Roboy/ss18_PocketBoy.git
```

2) Install [Unity 2018.2.12f1](https://unity3d.com/de/get-unity/download/archive)

3) Install Android SDK and setup the path in Unity to the SDK, click [here](https://docs.unity3d.com/Manual/android-sdksetup.html) for further instructions

4) Open the project in Unity

# Optinal: PocketBoy uses Google Cloud TTS, if you have a valid API key

4a) Create a TTS configuration file in the path shown in the picture to avoid publishing your key, as the contents of the folder are ignored by a local .gitignore

![create-tts](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/create-tts-config.png)
![tts-path](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/create-tts-config-path.png)

4b) Setup the TTS configuration file

![setup-tts](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/set-tts-config.png)

4c) Drag the TTS configuration file to the Roboy prefab

![roboy-path](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/roboy-prefab-path.png)
![roboy-tts](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/roboy-prefab.png)

# Build

5) Switch to Android platform in the build settings and press on build

![build](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/switch-platform.png)

6) If you have build errors due to some error from Gradle saying that the NDK bundle is missing, switch to internal

![build-internal](https://raw.githubusercontent.com/Roboy/ss18_PocketBoy/master/readme-resources/switch-platform-internal.png)