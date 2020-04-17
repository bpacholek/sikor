#!/bin/sh

if grep -q "it-works" "test.log"; then
  echo "The script ran ok"
  exit 0
else
  echo "The script failed" >&2
  exit 1
fi
