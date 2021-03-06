﻿using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ProductDao
    {
        OnlineShopDbContext db = null;
        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }
        public List<Product> ListAll()
        {
            return db.Products.OrderByDescending(m => m.CreatedDate).ToList();
        }
        public List<Product> ListByCategoryId(long categoryID)
        {
            return db.Products.Where(m=>m.CategoryID == categoryID).OrderByDescending(m => m.CreatedDate).ToList();
        }
        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Get list product by category
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public List<ProductViewModel> ListByCategoryId(long categoryID, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            totalRecord = db.Products.Where(x => x.CategoryID == categoryID).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.CategoryID == categoryID
                         select new
                         {
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             CateName = x.Name,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             Price = x.Price
                         });
            model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }
        public List<ProductViewModel> Search(string keyword, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            totalRecord = db.Products.Where(x => x.Name == keyword).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.Name.Contains(keyword)
                         select new
                         {
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             CateName = x.Name,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             Price = x.Price
                         });
            model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }
        /// <summary>
        /// List feature product
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Product> ListFeatureProduct(int top)
        {
            List<Product> result = new List<Product>();
            return db.OrderDetails.GroupBy(m => m.ProductID).OrderByDescending(x => x.Count()).Take(top).ToList().Select(m => new ProductDao().ViewDetail(m.Key)).ToList();
        }
        public List<Product> ListRelatedProducts(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }
        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }
        public void Create(string Name, string Description, string Image, Decimal Price, int Quantity, long CategoryID, string Detail, string CreatedBy, bool Status)
        {
            Product newProduct = new Product()
            {
                Name = Name,
                Description = Description,
                Image = Image,
                Price = Price,
                Quantity = Quantity,
                CategoryID = CategoryID,
                Detail = Detail,
                CreatedDate = DateTime.Now,
                CreatedBy = CreatedBy,
                Status = Status
            };
            db.Products.Add(newProduct);
            db.SaveChanges();
        }
        public void Edit(long ID,string Name, string Description, string Image, Decimal Price, int Quantity, long CategoryID, string Detail, string ModifiedBy, bool Status)
        {
            Product model = ViewDetail(ID);
            model.Name = Name;
            model.Description = Description;
            if(!string.IsNullOrEmpty(Image))
            {
                model.Image = Image;
            }
            model.Price = Price;
            model.Quantity = Quantity;
            model.CategoryID = CategoryID;
            model.Detail = Detail;
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = ModifiedBy;
            model.Status = Status;
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(long id)
        {
            Product model = db.Products.Find(id);
            if(model != null)
            {
                db.Products.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
