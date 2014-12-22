#!/usr/bin/env bash

PATH="/usr/local/bin:$PATH"

die () {
  echo "$1"
  exit 1
}

pushd build/xcode
  BUILT_PRODUCTS_DIR="$(xcodebuild -showBuildSettings | grep -i " BUILT_PRODUCTS_DIR" | sed -e 's/^\([^=]*= \)//')"
  ATTACHED_UDIDS=$(system_profiler SPUSBDataType | sed -n -E '/(iPhone|iPad)/,/Serial/p' | grep "Serial Number:" | awk -F ": " '{print $2}')
  [ -z "$ATTACHED_UDIDS" ] && exit 0

  [ -z "$UNITY_BUILD_KEYCHAIN_PASSWORD" ] || security -v unlock-keychain -p "$UNITY_BUILD_KEYCHAIN_PASSWORD"

  xcodebuild \
    -sdk iphoneos \
    -project "Unity-iPhone.xcodeproj" \
    -scheme "Unity-iPhone" \
    -configuration "Debug" \
    -xcconfig "extra.xcconfig" \
    clean \
    build

  pushd "$BUILT_PRODUCTS_DIR"
    for UDID in $ATTACHED_UDIDS; do
      ideviceinstaller --udid "$UDID" --install *.app
    done
  popd
popd
