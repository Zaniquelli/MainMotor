@echo off
echo Configurando Docker para usar o daemon do Minikube...
@FOR /f "tokens=*" %%i IN ('minikube -p minikube docker-env --shell cmd') DO @%%i

echo Construindo a imagem no Minikube...
docker build -t mainmotor-api:latest .

echo Fazendo deploy com Helm...
helm upgrade --install mainmotor-api ./helm

echo Deploy concluído!
kubectl get pods

echo.
echo Para verificar logs: kubectl logs -f deployment/mainmotor-api
echo Para acessar a API: minikube service mainmotor-api --url