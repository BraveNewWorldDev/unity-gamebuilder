#!/usr/bin/env bash

# Builds Unity Projects for ALL platforms.
# Expected to be run from a project's root directory.
#
# OSX:     NAME-osx.zip
# Windows: NAME-windows.zip
# Linux:   NAME-linux.tar.gz
# Web:     /game/game.html /game/game.unity3d

# Change via ENV or directly below.
[ -z $UNITY_BUILD_LOG ] && UNITY_BUILD_LOG="/dev/stdout"
UNITY_BUILD_BIN="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

[ -z "$UNITY_BUILD_SETTINGS" ] || cp ".Settings/${UNITY_BUILD_SETTINGS}.cs" Assets/Scripts/Settings.cs
[ -z "$UNITY_BUILD_INFO" ] || sed -i '' -e "s,__BUILD_INFO__,\"$UNITY_BUILD_INFO\"," Assets/Scripts/Settings.cs
unity_args="-batchmode -nographics -quit -projectPath \"$PWD\" -logFile \"$UNITY_BUILD_LOG\" -executeMethod GameBuilder.Build"

#echo $UNITY_BUILD_BIN
#echo $unity_args | xargs -0
#false
eval "$UNITY_BUILD_BIN $unity_args"

if [ $? != 0 ]; then
  echo "Build Failed!"
  exit 1
fi
