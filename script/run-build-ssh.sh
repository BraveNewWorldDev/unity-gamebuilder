#!/usr/bin/env bash

die () {
  echo "$1"
  exit 1
}

REMOTE_PATH="/tmp/unity-build-$(date +%Y%m%d%H%M%S)"
REMOTE_HOST="$1"

[ -z "$REMOTE_HOST" ] && die "Usage: rui-build-ssh.sh REMOTE_HOST"

echo "Copying files to remote host"
rsync -a -e ssh --exclude '*.git' --exclude 'build/*' . $REMOTE_HOST:"$REMOTE_PATH"

git_rev=$(git rev-parse HEAD)
build_time=$(date "+%H:%M:%S %Y/%m/%d")
UNITY_BUILD_INFO="$build_time - ${git_rev:0:8}"

set | grep -e UNITY -e APP > environment
rsync -e ssh environment $REMOTE_HOST:"$REMOTE_PATH"

echo "Building Unity Project"
ssh $REMOTE_HOST "cd \"$REMOTE_PATH\" && foreman run -e environment Assets/GameBuilder/script/run-build.sh"
#ssh $REMOTE_HOST "cd \"$REMOTE_PATH\" && dotenv env"
if [ $? != 0 ]; then
  echo "Build Failed!"
  rsync -a -e ssh $REMOTE_HOST:"$REMOTE_PATH/build/log.txt" build/
  exit 1
fi

echo "Build was successful, copying files back."
rsync -a -e ssh $REMOTE_HOST:"$REMOTE_PATH/build/" build/

rm environment
