#!/usr/bin/env bash

awslocal dynamodb create-table \
   --table-name HangfireJobStorage \
   --attribute-definitions AttributeName=Id,AttributeType=S \
   --key-schema AttributeName=Id,KeyType=HASH \
   --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
   --region "us-east-1"


awslocal dynamodb update-time-to-live --table-name HangfireJobStorage \
   --time-to-live-specification Enabled=true,AttributeName=ExpirationTimestamp \
   --region "us-east-1"