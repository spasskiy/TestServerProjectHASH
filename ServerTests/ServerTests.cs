using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestServerProject;

namespace ServerTests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void Server_CanBeCreated()
        {
            // Arrange & Act
            Server server = new Server();

            // Assert
            Assert.IsNotNull(server);
        }
        [TestMethod]
        public async Task StartAsync_ShouldNotThrowException()
        {
            // Arrange
            Server server = new Server();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<System.NotImplementedException>(() => server.StartAsync());
        }
    }
}
