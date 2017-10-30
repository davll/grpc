mkdir -p .build-android-armv7
cd .build-android-armv7
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/android.toolchain.cmake \
      -DANDROID_NDK=$ANDROID_NDK_ROOT                \
      -DCMAKE_BUILD_TYPE=Release                     \
      -DANDROID_ABI="armeabi-v7a with NEON"          \
      -DANDROID_NATIVE_API_LEVEL=23                  \
      -DBENCHMARK_ENABLE_TESTING=NO                  \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      -DgRPC_USE_BENCHMARK=OFF                       \
      ..
make grpc_csharp_ext
cd ..

mkdir -p .build-android-x86
cd .build-android-x86
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/android.toolchain.cmake \
      -DANDROID_NDK=$ANDROID_NDK_ROOT                \
      -DCMAKE_BUILD_TYPE=Release                     \
      -DANDROID_ABI="x86"                            \
      -DANDROID_NATIVE_API_LEVEL=23                  \
      -DBENCHMARK_ENABLE_TESTING=NO                  \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      -DgRPC_USE_BENCHMARK=OFF                       \
      ..
make grpc_csharp_ext
cd ..
