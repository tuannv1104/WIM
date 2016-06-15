
namespace WIM_MVC.Models.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class WIMEntities : DbContext
    {
        public WIMEntities(string connection)
            : base(connection)
        {
        }
    }
}