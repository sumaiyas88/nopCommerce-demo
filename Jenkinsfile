pipeline {
    agent { label 'Dotnet9' }

    options {
        timeout(time: 1, unit: 'HOURS')  // Use uppercase for unit
    }

    triggers {
        pollSCM('*/5 * * * *')
    }

    tools {
        dotnetsdk 'DOTNET9'  // Requires .NET SDK Support plugin and configured tool
    }

    stages {
        stage('SCM') {  // Stage names must be strings
            steps {
                git url: 'https://github.com/sumaiyas88/nopCommerce-demo.git',
                    branch: 'develop'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet restore src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'dotnet build -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'mkdir -p published && dotnet publish -o ./published -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
            }
        }
    }

    post {
        success {
            // Zip the published folder (requires Pipeline Utility Steps plugin)
            zip zipFile: 'published.zip', dir: 'published'

            // Archive the zip as a build artifact
            archiveArtifacts artifacts: 'published.zip', fingerprint: true
        }
    }
}
