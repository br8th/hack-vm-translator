all : publish

clean:
	dotnet clean
	rm VMTranslator

publish:
	# Create HackVMTranslator executable in cwd dir
	dotnet publish -c Release --os linux -o Release -f netcoreapp3.1 --self-contained true
	echo "Making VMTranslator executable..."
	echo "#!/bin/bash\n" > VMTranslator && echo 'Release/HackVMTranslator $$1' >> VMTranslator
	chmod +x VMTranslator