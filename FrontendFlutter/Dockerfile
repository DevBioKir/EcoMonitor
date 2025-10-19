FROM cirrusci/flutter:latest

#acceptance of all Android SDK licenses
RUN yes | sdkmanger --licenses

#installing the required Android SDK components
RUN sdkmanager "platform-tools" "platforms;android-30" "build-tools;30.0.3"

#setting environment variables for the Android SDK
ENV ANDROID_SDK_ROOT=/sdk
ENV PATH="${PATH}:${ANDROID_SDK_ROOT}/platform-tools"

#work dir the project
WORKDIR /app

#startup bush
CMD ["bash"] 