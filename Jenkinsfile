pipeline{
    agent any
    options{
        disableConcurrentBuilds()
        timestamps()
    }
    stages{
        stage(Setup .NET SDK){
            steps{
                sh 'dotnet --version'
            }
        }
    }
}