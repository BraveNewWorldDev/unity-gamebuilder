#!/usr/bin/env bash

SDK_ROOT=/opt/adt/latest/sdk
ADB="$SDK_ROOT/platform-tools/adb"

function adb-run-all {
  for SERIAL in $("$ADB" devices | grep -v List | cut -f 1); do
    echo "$SERIAL"
    "$ADB" -s $SERIAL "$@" &
  done

  for job in $(jobs -p); do
    wait $job
  done
}

adb-run-all install -r build/"$UNITY_BUILD_NAME".apk
[ -z "$UNITY_BUILD_ANDROID_ACTIVITY" ] || adb-run-all shell am start -a android.intent.action.MAIN -n "$UNITY_BUILD_ANDROID_ACTIVITY"
