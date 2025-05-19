using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WH.Models; 

namespace WH.DataContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext():base(nameOrConnectionString: "MyConnection")
        {

        } 
        public virtual DbSet<UserInfoClass> UserInfoObj { get; set; }
        public virtual DbSet<ChiTietPhieuNhapClass> ChiTietPhieuNhapObj { get; set; }
        public virtual DbSet<ChiTietPhieuXuatClass> ChiTietPhieuXuatObj { get; set; }
        public virtual DbSet<DmvtClass> DmvtObj { get; set; }
        public virtual DbSet<DmdvtClass> DmdvtObj { get; set; }
        public virtual DbSet<DmkhClass> DmkhObj { get; set; }
        public virtual DbSet<DmkhoClass> DmkhoObj { get; set; }
        public virtual DbSet<DmnccClass> DmnccObj { get; set; }
        public virtual DbSet<LoaiVatTuClass> LoaiVatTuObj { get; set; }
        public virtual DbSet<PhieuNhapClass> PhieuNhapObj { get; set; }
        public virtual DbSet<PhieuXuatClass> PhieuXuatObj { get; set; }
        public virtual DbSet<TonKhoClass> TonKhoObj { get; set; }
        public virtual DbSet<DmqddvtClass> DmqddvtObj { get; set; }




    }
}