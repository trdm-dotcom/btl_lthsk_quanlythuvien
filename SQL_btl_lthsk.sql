
ALTER TABLE tblSach ALTER COLUMN sTenS nvarchar(255);
ALTER TABLE tblSinhvien ALTER COLUMN sTenSV nvarchar(255);
ALTER TABLE tblThuthu ALTER COLUMN sTenTT nvarchar(255);
ALTER TABLE tblTheloai ALTER COLUMN sTenL nvarchar(255);
ALTER TABLE tblSinhvien ALTER COLUMN sLop varchar(10);
ALTER TABLE tblPhieumuon ADD iTrangthai int
ALTER TABLE tblThuthu ADD sMatkhau varchar(255)
ALTER TABLE tblThuthu ADD iQuyen int
ALTER TABLE tblSinhvien ADD iTrangthai int
ALTER TABLE tblPhieumuon ADD dNgayMuon date

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
		WHEN iTrangthai = 0 THEN N'Khóa'
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

create proc getSach
as
Begin
	Select sMaS, sTenS, iSoLuong, tblTheloai.sMaL, sTenL,
		(Select count(tblPhieumuon.sMaPhieu) from tblPhieumuonchitiet 
		inner join tblPhieumuon 
		on tblPhieumuonchitiet.sMaPhieu = tblPhieumuon.sMaPhieu
		where tblPhieumuonchitiet.sMaS = tblSach.sMaS and tblPhieumuon.iTrangthai = 0) as duocmuon
	from tblSach 
	inner join tblTheloai
	on tblSach.sMaL = tblTheloai.sMaL
	ORDER BY sTenS, sTenL ASC
End

create proc getPhieumuonBySach
@mas nvarchar(10)
as
Begin
	Select sTenSV,tblPhieumuon.sMaPhieu, dNgayMuon
	from tblSinhvien
	inner join tblPhieumuon
	on tblSinhvien.sMaSV = tblPhieumuon.sMaSV
	inner join tblPhieumuonchitiet
	on tblPhieumuon.sMaPhieu = tblPhieumuonchitiet.sMaPhieu
	where sMaS = @mas
End

create proc doLogin
@matt nvarchar(10),
@pass varchar(255)
as
Begin
	Select * from tblThuthu where sMaTT = @matt and sMatkhau = @pass
End


create proc getPhieu
as
Begin
	Select  tblPhieumuon.sMaPhieu,tblThuthu.sMaTT,tblSinhvien.sMaSV,dNgayMuon,sTenTT,sTenSV,tblPhieuMuon.iTrangthai,CASE WHEN tblPhieuMuon.iTrangthai = 1 THEN N'Đã trả' ELSE N'Chưa trả' END as trangthai  from tblPhieumuon 
	inner join tblSinhvien
	on tblPhieumuon.sMaSV = tblSinhvien.sMaSV
	inner join tblThuthu
	on tblPhieumuon.sMaTT = tblThuthu.sMaTT
	Order By dNgayMuon DESC
End

create proc doChangePass
@matt nvarchar(10),
@pass varchar(255),
@new varchar(255)
as
begin
	if EXISTS (SELECT * FROM tblThuthu WHERE sMaTT = @matt AND sMatkhau = @pass)
		begin
			Begin TRAN
				Begin TRY
					update tblThuthu set sMatkhau = @new where sMaTT = @matt
					COMMIT TRANSACTION
				End Try
			Begin CATCH
				ROLLBACK TRANSACTION
			End CATCH
		end
end


create proc insertPhieumuon
@mapm nvarchar(10),
@matt nvarchar(10),
@masv nvarchar(10),
@dngaymuon date
as
begin
Begin TRAN
	Begin TRY
			insert into tblPhieumuon(sMaPhieu,sMaTT,sMaSV,iTrangthai,dNgayMuon) values(@mapm,@matt,@masv,0,@dngaymuon)
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
end

create proc insertChitiet
@mapm nvarchar(10),
@mas nvarchar(10)
as
begin
	if EXISTS (SELECT * FROM tblPhieumuon WHERE sMaPhieu = @mapm)
		begin
			Begin TRAN
				Begin TRY
					insert into tblPhieumuonchitiet(sMaPhieu,sMaS) values(@mapm,@mas)
					COMMIT TRANSACTION
				End Try
			Begin CATCH
				ROLLBACK TRANSACTION
			End CATCH
		end
end

create proc getChitiet
@mapm nvarchar(10)
as
begin
	select * from tblPhieumuonchitiet where sMaPhieu = @mapm
end

create proc traSach
@mapm nvarchar(10)
as
begin
	Begin TRAN
		Begin TRY
			UPDATE tblPhieumuon SET iTrangthai = 1 WHERE sMaPhieu = @mapm
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
end

create proc updateSach
@mas nvarchar(10),
@tens nvarchar(255),
@mal nvarchar(10),
@sl int
as
Begin
	Begin TRAN
		Begin TRY
			update tblSach set sTenS = @tens, iSoLuong = @sl, sMaL = @mal where sMaS = @mas 
			COMMIT TRANSACTION
		End Try
	Begin CATCH
		ROLLBACK TRANSACTION
	End CATCH
End