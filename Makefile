.PHONY: backend-build backend-run backend-stop flutter-run flutter-build flutter-clean backend-clean

#Backend build and startup
backend-build:
	docker build -t ecomonitor-backend ./Backend
	
backend-run:
	docker run -d -p 8080:80 --name backend-container ecomonitor-backend

backend-stop:
	docker stop backend-container || true
	docker rm backend-container || true
	
backend-clean: backend-stop
	docker image rm ecomonitor-backend || true


flutter-run:
	docker run --rm -it \
		-v $(PWD)/frontend:/app \
		-w /app \
		cirrusci/flutter:latest flutter build apk --release
		
flutter-clean:
	docker run -rm -it \
		-v $(PWD)/frontend:/app \
		-w /app \
		cirrusci/flutter:latest flutter clean