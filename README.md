# GameBuilder for Unity

A collection of scripts to build Unity projects in a CI environment.

## Installation

> **NOTE:** This package includes MiniJSON and XCodeEditor-for-Unity.

Download this repository and move the `GameBuilder/` directory to
your project's `Assets/` directory.

To build:

```bash
./Assets/GameBuilder/script/run-build.sh
```

If you would like to run the build on another machine:

```bash
./Assets/GameBuilder/script/run-build-ssh.sh user@hostname
```

Note that in using `run-build-ssh.sh` you will need:

- ssh
- rsync
- foreman (on the build machine)

## Configuration

All configuration is done via Environment Variables.

Variable Name | Description
--------------|------------
`UNITY_BUILD_NAME` | The name to be used in the generated archive files (**required**).
`UNITY_BUILD_TARGETS` | Space-separated list of targets to build. If empty it is assumed to be `all`.
`UNITY_BUILD_EXCLUDE` | Space-separated list of targets to *exclude*.
`UNITY_AFTER_OSX` | Run this script after the build for this platform has finished.
`UNITY_AFTER_WINDOWS` | ""
`UNITY_AFTER_LINUX` | ""
`UNITY_AFTER_WEB` | ""
`UNITY_AFTER_WEBGL` | ""
`UNITY_AFTER_IOS` | ""
`UNITY_AFTER_ANDROID` | ""

## Helper Scripts

### `ios-push-to-all.sh`

Variable Name | Description
--------------|------------
`UNITY_BUILD_KEYCHAIN_PASSWORD` | Set this to unlock the keychain. [More info](http://stackoverflow.com/questions/20205162/user-interaction-is-not-allowed-trying-to-sign-an-osx-app-using-codesign)

Push resulting .ipa to all connected iOS devices.

### `android-push-to-all.sh`

Variable Name | Description
--------------|------------
`UNITY_BUILD_ANDROID_ACTIVITY` | Set this to have your application launch once pushed.

Push resulting .apk to all connected Android devices.

### `android-sign-zipalign.sh`

Variable Name | Description
--------------|------------
`UNITY_KEYSTORE_NAME` | The path to your keystore. Defaults to `android.keystore`.
`UNITY_KEYALIAS_NAME` | The name of the alias. Defaults to `UNITY_BUILD_NAME`.
`UNITY_KEYSTORE_PASSWORD` | The password for the keystore (**required**).
`UNITY_KEYALIAS_PASSWORD` | The password for the alias (**required**).

Sign and zipalign the .apk.
