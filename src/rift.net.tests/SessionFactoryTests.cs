using NUnit.Framework;
using System;

namespace rift.net.tests
{
	[TestFixture ()]
	public class SessionFactoryTests
	{
		[Test ()]
		public void TestCase ()
		{
			var username = System.Configuration.ConfigurationManager.AppSettings ["username"];
			var password = System.Configuration.ConfigurationManager.AppSettings ["password"];

			var security = new SessionFactory ();

			var session = security.Login (username, password);

			Assert.That (session, Is.Not.Null);
			Assert.That (session.Id, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void TestLoginFailure()
		{
			var security = new SessionFactory ();

			Assert.That ( () => security.Login ("foo@bar.com", "foo"), Throws.TypeOf<AuthenticationException>());
		}
	}
}

