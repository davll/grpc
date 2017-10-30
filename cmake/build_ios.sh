mkdir -p .build-ios
cd .build-ios
cmake -DCMAKE_TOOLCHAIN_FILE="cmake/ios.toolchain.cmake" \
      -DIOS_PLATFORM=OS \
      ..
make
