ifndef UNITY_APP
	UNITY_APP="C:/Program Files/Unity/Hub/Editor/2019.4.13f1/Editor/Unity.exe"
endif

doc-build: doc-metadata
	docfx build doc_source/docfx.json -t statictoc

doc-metadata:
	docfx metadata doc_source/docfx.json
	sh ./sync_doc.sh

format: rebuild-project
	cd Unity && dotnet-format -w Unity.sln

rebuild-project:
	echo ${PWD}
	rm -f ./Unity/*.csproj
	rm -f ./Unity/*.sln
	rm -rf ./Unity/Library/ScriptAssemblies
	${UNITY_APP}  -quit -batchmode -projectPath "${PWD}/Unity" -executeMethod "ILib.SolutionSync.Run"

doc-release: rebuild-project doc-metadata
	rm -rf _site
	rm -rf docs
	docfx build doc_source/docfx.json
	cp -r _site docs
