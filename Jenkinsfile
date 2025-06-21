pipeline {
    agent { label 'Dotnet9' }
    options {
        timeout(time: 1, unit: 'HOURS')
    }
    triggers {
        pollSCM('*/5 * * * *')
    }
    tools {
        dotnetsdk 'DOTNET9'
    }
    stages {
        stage('SCM') {
            steps {
                git url: 'https://github.com/sumaiyas88/nopCommerce-demo.git',
                    branch: 'develop'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet restore src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'dotnet build -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'mkdir -p published1 && dotnet publish -o ./published1 -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
            }
        }
	}
        post {
            success {
            zip zipFile: './published1.zip',
                archive: true,
                dir: './published1'
            }
        }
}