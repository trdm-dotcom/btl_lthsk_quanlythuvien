ALTER TABLE tblSach ALTER COLUMN sTenS nvarchar(255);
ALTER TABLE tblSinhvien ALTER COLUMN sTenSV nvarchar(255);
ALTER TABLE tblThuthu ALTER COLUMN sTenTT nvarchar(255);
ALTER TABLE tblTheloai ALTER COLUMN sTenL nvarchar(255);
ALTER TABLE tblSinhvien ALTER COLUMN sLop varchar(10);
ALTER TABLE tblPhieumuon ADD iTrangthai int
ALTER TABLE tblThuthu ADD sMatkhau varchar(255)
ALTER TABLE tblThuthu ADD iQuyen int
ALTER TABLE tblSinhvien ADD iTrangthai int


create table tblTaikhoan(
	sMaTK nvarchar(10) NOT NULL,
	sTaikhoan nvarchar(255) NOT NULL UNIQUE,
	sMatkhau varchar(255) NOT NULL,
	sMaTT nvarchar(10) NOT NULL,
	sMaQuyen int NOT NULL,
	CONSTRAINT PK_tblTaikhoan PRIMARY KEY (sMaTK),
	CONSTRAINT PK_tblThuthu_tblTaikhoan FOREIGN KEY (sMaTT) REFERENCES tblThuthu(sMaTT)
)

create proc insertTT
@matt nvarchar(10),
@hoten nvarchar(255),
@matkhau varchar(255),
@maquyen int
as
Begin
	Begin TRAN
		Begin TRY
			insert into tblThuthu(sMaTT,sTenTT,sMatkhau,iQuyen) values(@matt,@hoten,@matkhau,@maquyen)
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc updateTT
@matt nvarchar(10),
@hoten nvarchar(255),
@matkhau varchar(255)
as
Begin
	Begin TRAN
		Begin TRY
			IF @matkhau IS NULL or @matkhau = ''
				UPDATE tblThuthu SET sTenTT = @hoten WHERE sMaTT = @matt
			ELSE
				UPDATE tblThuthu SET sTenTT = @hoten, sMatkhau = @matkhau WHERE sMaTT = @matt
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc insertTheloai
@matl nvarchar(10),
@tentl nvarchar(255)
as
Begin
	Begin TRAN
		Begin TRY
			insert into tblTheloai (sMaL,sTenL) values(@matl,@tentl)
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc insertSach
@mas nvarchar(10),
@tens nvarchar(255),
@mal nvarchar(10),
@sl int
as
Begin
	Begin TRAN
		Begin TRY
			insert into tblSach (sMaS,sTenS,sMaL,iSoLuong) values(@mas,@tens,@mal,@sl)
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc getTheloai
as
Begin
	SELECT * FROM tblTheloai
End

create proc insertSV
@masv nvarchar(10),
@tensv nvarchar(255),
@lop varchar(10),
@trangthai int
as
Begin
	Begin TRAN
		Begin TRY
			insert into tblSinhvien (sMaSV,sTenSV,sLop,iTrangthai) values(@masv,@tensv,@lop,@trangthai)
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc updateSV
@masv nvarchar(10),
@tensv nvarchar(255),
@lop varchar(10)
as
Begin
	Begin TRAN
		Begin TRY
			update tblSinhvien set sTenSV = @tensv,sLop = @lop where sMaSV = @masv
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End

create proc getStudent
as
Begin
	SELECT sMaSV,sTenSV,sLop, 
	CASE 
		WHEN iTrangthai = 0 THEN 'Khóa'
		ELSE ''
	END AS trangthai
	FROM tblSinhvien ORDER BY sTenSV, sLop ASC
End

create proc bandStudent
@masv nvarchar(10)
as
Begin
	Begin TRAN
		Begin TRY
			update tblSinhvien set iTrangthai = 0 Where sMaSV = @masv
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
END