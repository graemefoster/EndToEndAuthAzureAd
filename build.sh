
#!/bin/bash

for csproj in ./test/**/*.csproj; do
    dotnet test --collect:"XPlat Code Coverage" --results-directory $COMMON_TESTRESULTSDIRECTORY --logger:trx $csproj
done
sp
for csproj in ./src/**/*.csproj; do
    csprojFile=${csproj##*/}
    csprojName=${csprojFile%.*}
    dotnet publish $csproj -p:Version=$BUILD_SEMVER -o $BUILD_ARTIFACTSTAGINGDIRECTORY/artifacts/$csprojName/
done

echo "Building to $BUILD_BINARIESDIRECTORY using version $BUILD_SEMVER"
                