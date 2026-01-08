using AppData.Context;
using AppData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    public static class DataExtensions
    {
        public static void AddDataExtensions(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DbContext, AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IDataRepository, DataRepository>();
        }

      
    }
}
