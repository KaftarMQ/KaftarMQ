Here's a simple markup that indicates the steps you've outlined:

```markdown
# Deployment Process

To deploy the application stack `kaftarmq`, follow these steps:

1. Build the Docker images with the following command:
   ```
docker-compose build
   ```

2. Deploy the stack using Docker Swarm with the command:
   ```
docker stack deploy -c docker-compose.yml kaftarmq
   ```

3. After the stack is successfully deployed and running, you can remove it using:
   ```
docker stack rm kaftarmq
   ```

By completing these steps, the running of the program is done.
```

This assumes that each command is run in sequence and that the previous command completes successfully before the next one is initiated. If there are specific conditions or configurations needed for these commands to work, they should be specified before running these commands.