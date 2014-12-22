#!/usr/bin/env bash

die () {
  echo "$1"
  exit 1
}

[ -z "$UNITY_BUILD_NAME" ] && die "Please set 'UNITY_BUILD_NAME'"

[ -z "$UNITY_KEYSTORE_NAME" ] && UNITY_KEYSTORE_NAME=android.keystore
[ -z "$UNITY_KEYSTORE_PASSWORD" ] && die "Please set 'UNITY_KEYSTORE_PASSWORD'"

[ -z "$UNITY_KEYALIAS_NAME" ] && UNITY_KEYALIAS_NAME="$UNITY_BUILD_NAME"
[ -z "$UNITY_KEYALIAS_PASSWORD" ] && die "Please set 'UNITY_KEYALIAS_PASSWORD'"

jarsigner \
  -keystore "$UNITY_KEYSTORE_NAME" \
  -storepass "$UNITY_KEYSTORE_PASSWORD" \
  -keypass "$UNITY_KEYALIAS_PASSWORD" \
  build/"$UNITY_BUILD_NAME".apk \
  "$UNITY_KEYALIAS_NAME"

/opt/adt/latest/sdk/build-tools/android-4.4W/zipalign \
  -f \
  4 \
  build/"$UNITY_BUILD_NAME".apk \
  build/"$UNITY_BUILD_NAME".aligned.apk

zip -d build/"$UNITY_BUILD_NAME".aligned.apk "META-INF*"
rm build/"$UNITY_BUILD_NAME".apk
mv build/"$UNITY_BUILD_NAME".aligned.apk build/"$UNITY_BUILD_NAME".apk
