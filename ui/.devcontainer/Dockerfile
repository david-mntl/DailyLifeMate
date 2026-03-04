FROM node:20-bookworm-slim

WORKDIR /workspace

# Install common tools (git is required for standard devcontainer features)
RUN apt-get update && apt-get install -y \
    vim \
    git \
    curl \
    unzip \
    iputils-ping \
    && rm -rf /var/lib/apt/lists/*

# The official Node image already has a 'node' user with UID/GID 1000.
# We just need to make sure this user owns the workspace directory.
RUN mkdir -p /workspace/src \
    && chown -R node:node /workspace

USER node

WORKDIR /workspace