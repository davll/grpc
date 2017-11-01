mkdir -p build-android/armv7
cd build-android/armv7
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/android.toolchain.cmake \
      -DANDROID_NDK=$ANDROID_NDK_ROOT                \
      -DCMAKE_BUILD_TYPE=Release                     \
      -DANDROID_ABI="armeabi-v7a with NEON"          \
      -DANDROID_NATIVE_API_LEVEL=23                  \
      -DgRPC_ZLIB_PROVIDER=package                   \
      -DgRPC_BENCHMARK_PROVIDER=module               \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      ../..
make grpc_csharp_ext
cd ../..

mkdir -p build-android/x86
cd build-android/x86
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/android.toolchain.cmake \
      -DANDROID_NDK=$ANDROID_NDK_ROOT                \
      -DCMAKE_BUILD_TYPE=Release                     \
      -DANDROID_ABI="x86"                            \
      -DANDROID_NATIVE_API_LEVEL=23                  \
      -DgRPC_ZLIB_PROVIDER=package                   \
      -DgRPC_BENCHMARK_PROVIDER=module               \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      ../..
make grpc_csharp_ext
cd ../..
