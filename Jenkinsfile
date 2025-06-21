pipeline {
    agent { label 'dotnet9'}
    options {
        timeout(time: 1, unit: 'HOURS')
    }
    triggers {
        pollSCM('* * * * *')
    }
    tools {
        dotnetsdk('dotnet9')
    }
    stages {
        stage('SCM') {
            steps {
                git url: 'https://github.com/sumaiyas88/nopCommerce-demo.git',
                    branch: 'develop'
            }
        }
        stage('Build Dotnet') {
            steps {
                sh 'dotnet restore src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'dotnet build -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'mkdir -p published && dotnet publish -o ./published -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
            }
        }
    }
    post {
        success {
            zip zipFile: './published.zip',
                archive: true,
                dir: './published'
            }
    }
}