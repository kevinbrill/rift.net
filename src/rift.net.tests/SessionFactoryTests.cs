using NUnit.Framework;
using System;

namespace rift.net.tests
{
	[TestFixture ()]
	public class SessionFactoryTests
	{
		[Test ()]
		public void TestUserNamePasswordLogin ()
		{
			var username = System.Configuration.ConfigurationManager.AppSettings ["username"];
			var password = System.Configuration.ConfigurationManager.AppSettings ["password"];

			var security = new SessionFactory ();

			var session = security.Login (username, password);

			Assert.That (session, Is.Not.Null);
			Assert.That (session.Id, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void Ticket_Login_Should_Succeed()
		{
			var username = System.Configuration.ConfigurationManager.AppSettings ["username"];
			var password = System.Configuration.ConfigurationManager.AppSettings ["password"];

			var security = new SessionFactory ();

			var session = security.Login (username, password);

			Assume.That (session, Is.Not.Null);
			Assume.That (session.Ticket, Is.Not.Null.And.Not.Empty);

			session = security.Login(session.Ticket);

			Assert.That (session, Is.Not.Null);
			Assert.That (session.Id, Is.Not.Null.And.Not.Empty);
			Assert.That (session.Ticket, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void TestLoginFailure()
		{
			var security = new SessionFactory ();

			Assert.That ( () => security.Login ("foo@bar.com", "foo"), Throws.TypeOf<AuthenticationException>());
		}
	}
}