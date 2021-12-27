using MIWE.Data.Entities;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(WorkerContext context) : base(context)
        {

        }
    }
}
