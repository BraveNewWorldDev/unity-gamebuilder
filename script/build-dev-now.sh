#!/usr/bin/env bash

UNITY_BUILD_TARGETS="android ios" \
  UNITY_AFTER_IOS=./script/run-after-ios-dev.sh \
  UNITY_AFTER_ANDROID=./script/run-after-android-dev.sh \
  UNITY_BUILD_NAME="Unity Test" \
  ./script/run-build.sh
