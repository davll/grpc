mkdir -p .build-ios-armv7
cd .build-ios-armv7
cmake -DIOS_PLATFORM=OS \
      -DENABLE_BITCODE=FALSE \
      -DgRPC_USE_BENCHMARK=OFF \
      -DCMAKE_OSX_SYSROOT=iphoneos \
      -DCMAKE_OSX_ARCHITECTURES=armv7 \
      -DPERL_EXECUTABLE=/usr/bin/perl \
      -DGO_EXECUTABLE=/usr/local/bin/go \
      -DBENCHMARK_ENABLE_TESTING=NO                  \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      -DCMAKE_BUILD_TYPE=Release                     \
      ..
make grpc_csharp_ext
cd ..

mkdir -p .build-ios-armv7s
cd .build-ios-armv7s
cmake -DIOS_PLATFORM=OS \
      -DENABLE_BITCODE=FALSE \
      -DgRPC_USE_BENCHMARK=OFF \
      -DCMAKE_OSX_SYSROOT=iphoneos \
      -DCMAKE_OSX_ARCHITECTURES=armv7s \
      -DPERL_EXECUTABLE=/usr/bin/perl \
      -DGO_EXECUTABLE=/usr/local/bin/go \
      -DBENCHMARK_ENABLE_TESTING=NO                  \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      -DCMAKE_BUILD_TYPE=Release                     \
      ..
make grpc_csharp_ext
cd ..

mkdir -p .build-ios-arm64
cd .build-ios-arm64
cmake -DIOS_PLATFORM=OS \
      -DENABLE_BITCODE=FALSE \
      -DgRPC_USE_BENCHMARK=OFF \
      -DCMAKE_OSX_SYSROOT=iphoneos \
      -DCMAKE_OSX_ARCHITECTURES=arm64 \
      -DPERL_EXECUTABLE=/usr/bin/perl \
      -DGO_EXECUTABLE=/usr/local/bin/go \
      -DBENCHMARK_ENABLE_TESTING=NO                  \
      -DgRPC_BUILD_TESTS=NO                          \
      -Dprotobuf_BUILD_EXAMPLES=NO                   \
      -DCMAKE_BUILD_TYPE=Release                     \
      ..
make grpc_csharp_ext
cd ..

#lipo .build-ios-armv7/lib
