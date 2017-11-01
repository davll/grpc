mkdir -p build-ios/armv7
cd build-ios/armv7
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/ios.toolchain.cmake \
      -DIOS_DEPLOYMENT_TARGET=7.0                      \
      -DIOS_PLATFORM=OS                                \
      -DIOS_ARCH=armv7                                 \
      -DCMAKE_SYSTEM_PROCESSOR=arm                     \
      -DPERL_EXECUTABLE=/usr/bin/perl                  \
      -DGO_EXECUTABLE=/usr/local/bin/go                \
      -DgRPC_ZLIB_PROVIDER=package                     \
      -DgRPC_BENCHMARK_PROVIDER=module                 \
      -DgRPC_BUILD_TESTS=NO                            \
      -Dprotobuf_BUILD_EXAMPLES=NO                     \
      -DCMAKE_BUILD_TYPE=Release                       \
      ../..
make grpc_csharp_ext
cd ../..

mkdir -p build-ios/armv7s
cd build-ios/armv7s
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/ios.toolchain.cmake \
      -DIOS_DEPLOYMENT_TARGET=7.0                      \
      -DIOS_PLATFORM=OS                                \
      -DIOS_ARCH=armv7s                                \
      -DCMAKE_SYSTEM_PROCESSOR=arm                     \
      -DPERL_EXECUTABLE=/usr/bin/perl                  \
      -DGO_EXECUTABLE=/usr/local/bin/go                \
      -DgRPC_ZLIB_PROVIDER=package                     \
      -DgRPC_BENCHMARK_PROVIDER=module                 \
      -DgRPC_BUILD_TESTS=NO                            \
      -Dprotobuf_BUILD_EXAMPLES=NO                     \
      -DCMAKE_BUILD_TYPE=Release                       \
      ../..
make grpc_csharp_ext
cd ../..

mkdir -p build-ios/arm64
cd build-ios/arm64
cmake -DCMAKE_TOOLCHAIN_FILE=cmake/ios.toolchain.cmake \
      -DIOS_DEPLOYMENT_TARGET=7.0                      \
      -DIOS_PLATFORM=OS                                \
      -DIOS_ARCH=arm64                                 \
      -DCMAKE_SYSTEM_PROCESSOR=aarch64                  \
      -DPERL_EXECUTABLE=/usr/bin/perl                  \
      -DGO_EXECUTABLE=/usr/local/bin/go                \
      -DgRPC_ZLIB_PROVIDER=package                     \
      -DgRPC_BENCHMARK_PROVIDER=module                 \
      -DgRPC_BUILD_TESTS=NO                            \
      -Dprotobuf_BUILD_EXAMPLES=NO                     \
      -DCMAKE_BUILD_TYPE=Release                       \
      ../..
make grpc_csharp_ext
cd ../..

mkdir -p build-ios
cd build-ios

lipo armv7/libgpr.a armv7s/libgpr.a arm64/libgpr.a \
     -create -output libgpr.a

lipo armv7/libgrpc.a armv7s/libgrpc.a arm64/libgrpc.a \
     -create -output libgrpc.a

lipo armv7/libgrpc_csharp_ext.a armv7s/libgrpc_csharp_ext.a arm64/libgrpc_csharp_ext.a \
     -create -output libgrpc_csharp_ext.a

lipo armv7/third_party/boringssl/crypto/libcrypto.a \
     armv7s/third_party/boringssl/crypto/libcrypto.a \
     arm64/third_party/boringssl/crypto/libcrypto.a \
     -create -output libcrypto.a

lipo armv7/third_party/boringssl/ssl/libssl.a \
     armv7s/third_party/boringssl/ssl/libssl.a \
     arm64/third_party/boringssl/ssl/libssl.a \
     -create -output libssl.a

lipo armv7/third_party/cares/cares/lib/libcares.a \
     armv7s/third_party/cares/cares/lib/libcares.a \
     arm64/third_party/cares/cares/lib/libcares.a \
     -create -output libcares.a

#lipo armv7/third_party/zlib/libz.a \
#     armv7s/third_party/zlib/libz.a \
#     arm64/third_party/zlib/libz.a \
#     -create -output libz.a
