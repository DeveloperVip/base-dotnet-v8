pipeline {
    agent any

    environment {
        REMOTE_HOST = '192.168.205.109' 
        CREDENTIAL_ID = 'ssh-109'
        PROJECT_DIR = '/home/attt/JOBFAIR/BE'
        SOURCE_DIR = '/home/attt/JOBFAIR/BE/haui-htdn-be'
        DOCKER_COMPOSE = '/home/attt/JOBFAIR/BE/docker-compose.yml'
    }

    stages {
	
        stage('Check Connection & Source Directory') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: env.CREDENTIAL_ID, usernameVariable: 'SSH_USER', passwordVariable: 'SSH_PASSWORD')]) {
                        def remoteServer = [
                            name: 'target-backend-server',
                            host: env.REMOTE_HOST,
                            user: SSH_USER,
                            password: SSH_PASSWORD,
                            allowAnyHosts: true
                        ]
                        echo "--- Bắt đầu kiểm tra kết nối SSH và thư mục mã nguồn ---"
                        sshCommand remote: remoteServer, command: """
                            whoami # Kiểm tra người dùng hiện tại
                            cd ${env.SOURCE_DIR} || { echo "ERROR: Không tìm thấy thư mục mã nguồn: ${env.SOURCE_DIR}"; exit 1; }
                            echo "Thư mục mã nguồn đã tồn tại: ${env.SOURCE_DIR}"
                        """
                        echo "--- Kiểm tra kết nối và thư mục mã nguồn hoàn tất ---"
                    }
                }
            }
        }

        stage('Docker Compose Down') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: env.CREDENTIAL_ID, usernameVariable: 'SSH_USER', passwordVariable: 'SSH_PASSWORD')]) {
                        def remoteServer = [
                            name: 'target-backend-server',
                            host: env.REMOTE_HOST,
                            user: SSH_USER,
                            password: SSH_PASSWORD,
                            allowAnyHosts: true
                        ]
                        echo "--- Đang dừng các dịch vụ Docker Compose hiện có (nếu có) ---"
                        sshCommand remote: remoteServer, command: """
                            cd ${env.PROJECT_DIR} || { echo "ERROR: Không tìm thấy thư mục dự án: ${env.PROJECT_DIR}"; exit 1; }
                            sudo docker compose -f ${env.DOCKER_COMPOSE} down
                        """
                        echo "--- Dừng Docker Compose hoàn tất ---"
                    }
                }
            }
        }

        stage('Pull Code') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: env.CREDENTIAL_ID, usernameVariable: 'SSH_USER', passwordVariable: 'SSH_PASSWORD')]) {
                        def remoteServer = [
                            name: 'target-backend-server',
                            host: env.REMOTE_HOST,
                            user: SSH_USER,
                            password: SSH_PASSWORD,
                            allowAnyHosts: true
                        ]
                        echo "--- Đang kéo mã nguồn mới nhất từ Git ---"
                        sshCommand remote: remoteServer, command: """
                            cd ${env.SOURCE_DIR} && sudo git pull || { echo "ERROR: Kéo mã nguồn thất bại"; exit 1; }
                            echo "Mã nguồn đã được kéo thành công."
                        """
                        echo "--- Kéo mã nguồn hoàn tất ---"
                    }
                }
            }
        }

        stage('Build Inside Container') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: env.CREDENTIAL_ID, usernameVariable: 'SSH_USER', passwordVariable: 'SSH_PASSWORD')]) {
                        def remoteServer = [
                            name: 'target-backend-server',
                            host: env.REMOTE_HOST,
                            user: SSH_USER,
                            password: SSH_PASSWORD,
                            allowAnyHosts: true
                        ]
                        echo "--- Đang build ứng dụng bên trong container 'be-gqyc-builder-1' ---"
                        sshCommand remote: remoteServer, command: """
                            sudo docker exec be-builder-1 bash -c "cd /source && dotnet restore && dotnet publish -c Release -o /app" || {
                                echo "ERROR: Build ứng dụng thất bại bên trong container"; exit 1;
                            }
                            echo "Ứng dụng đã được build thành công."
                        """
                        echo "--- Build ứng dụng hoàn tất ---"
                    }
                }
            }
        }

        stage('Docker Compose Up') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: env.CREDENTIAL_ID, usernameVariable: 'SSH_USER', passwordVariable: 'SSH_PASSWORD')]) {
                        def remoteServer = [
                            name: 'target-backend-server',
                            host: env.REMOTE_HOST,
                            user: SSH_USER,
                            password: SSH_PASSWORD,
                            allowAnyHosts: true
                        ]
                        echo "--- Đang khởi động lại các dịch vụ Docker Compose ---"
                        sshCommand remote: remoteServer, command: """
                            cd ${env.PROJECT_DIR} || { echo "ERROR: Không tìm thấy thư mục dự án: ${env.PROJECT_DIR}"; exit 1; }
                            sudo docker compose -f ${env.DOCKER_COMPOSE} up -d
                            echo "Các dịch vụ Backend đã được khởi động và đang hoạt động."
                        """
                        echo "--- Khởi động Docker Compose hoàn tất ---"
                    }
                }
            }
        }
    }
}