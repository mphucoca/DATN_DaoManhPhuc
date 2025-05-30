--
-- PostgreSQL database dump
--

-- Dumped from database version 9.3.5
-- Dumped by pg_dump version 9.3.5
-- Started on 2025-05-30 23:44:04

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- TOC entry 208 (class 3079 OID 11750)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2097 (class 0 OID 0)
-- Dependencies: 208
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 571 (class 1247 OID 66103)
-- Name: bc_tonghop_phieu_nhap_result; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE bc_tonghop_phieu_nhap_result AS (
	stt double precision,
	sys_order double precision,
	sys_print double precision,
	sys_total double precision,
	so_ct character varying,
	ngay_ct timestamp without time zone,
	ma_kho character varying,
	ten_kho character varying,
	ma_ncc character varying,
	ten_ncc text,
	ma_vt character varying,
	ten_vt character varying,
	ma_dvt character varying,
	ten_dvt character varying,
	so_luong_nhap double precision,
	don_gia_nhap numeric,
	thanh_tien numeric
);


ALTER TYPE public.bc_tonghop_phieu_nhap_result OWNER TO postgres;

--
-- TOC entry 628 (class 1247 OID 66114)
-- Name: bc_tonghop_phieu_xuat_result; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE bc_tonghop_phieu_xuat_result AS (
	stt double precision,
	sys_order double precision,
	sys_print double precision,
	sys_total double precision,
	so_ct character varying,
	ngay_ct timestamp without time zone,
	ma_kho character varying,
	ten_kho character varying,
	ma_kh character varying,
	ten_kh text,
	ma_vt character varying,
	ten_vt character varying,
	ma_dvt character varying,
	ten_dvt character varying,
	so_luong_xuat double precision,
	don_gia_xuat numeric,
	thanh_tien numeric
);


ALTER TYPE public.bc_tonghop_phieu_xuat_result OWNER TO postgres;

--
-- TOC entry 568 (class 1247 OID 49388)
-- Name: bctonghopphieunhap_result; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE bctonghopphieunhap_result AS (
	sys_order integer,
	sys_print integer,
	sys_total integer,
	so_ct character varying,
	ngay_ct timestamp without time zone,
	ma_ncc character varying,
	ten_ncc character varying,
	ma_kho character varying,
	ten_kho character varying,
	ma_vt character varying,
	ten_vt character varying,
	ma_dvt character varying,
	ten_dvt character varying,
	so_luong_nhap integer,
	don_gia_nhap numeric(10,2),
	thanh_tien numeric(12,2),
	ghi_chu character varying
);


ALTER TYPE public.bctonghopphieunhap_result OWNER TO postgres;

--
-- TOC entry 631 (class 1247 OID 41812)
-- Name: bctontheokho_result; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE bctontheokho_result AS (
	stt double precision,
	sys_order double precision,
	sys_print double precision,
	sys_total double precision,
	ma_kho character varying,
	ten_kho character varying,
	mo_ta_kho character varying,
	dia_chi_kho character varying,
	ma_vt character varying,
	ten_vt character varying,
	rong double precision,
	cao double precision,
	khoi_luong double precision,
	mau_sac character varying,
	kieu_dang character varying,
	ma_dvt character varying,
	ten_dvt character varying,
	mo_ta_dvt character varying,
	ty_le_quy_doi double precision,
	so_luong_ton double precision,
	ngay_cap_nhat timestamp without time zone
);


ALTER TYPE public.bctontheokho_result OWNER TO postgres;

--
-- TOC entry 565 (class 1247 OID 90132)
-- Name: cbtontheokho_result; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE cbtontheokho_result AS (
	stt double precision,
	sys_order double precision,
	sys_print double precision,
	sys_total double precision,
	ma_kho character varying,
	ten_kho character varying,
	mo_ta_kho character varying,
	dia_chi_kho character varying,
	ma_vt character varying,
	ten_vt character varying,
	rong double precision,
	cao double precision,
	khoi_luong double precision,
	mau_sac character varying,
	kieu_dang character varying,
	ma_dvt character varying,
	ten_dvt character varying,
	mo_ta_dvt character varying,
	ty_le_quy_doi double precision,
	so_luong_ton double precision,
	so_luong_dinh_xuat double precision,
	so_luong_dang_nhap double precision,
	canh_bao_ton_kho character varying,
	ngay_cap_nhat timestamp without time zone
);


ALTER TYPE public.cbtontheokho_result OWNER TO postgres;

--
-- TOC entry 226 (class 1255 OID 66104)
-- Name: bc_tonghop_phieu_nhap(json); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION bc_tonghop_phieu_nhap(input_json json) RETURNS SETOF bc_tonghop_phieu_nhap_result
    LANGUAGE plpgsql
    AS $$
DECLARE
    ds_ma_kho text := input_json->>'ds_ma_kho';
    ds_ma_vt text := input_json->>'ds_ma_vt';
    search_keyword text := lower(trim(input_json->>'search_keyword'));
    tu_ngay timestamp := (input_json->>'tu_ngay')::timestamp;
    den_ngay timestamp := (input_json->>'den_ngay')::timestamp;
    r bc_tonghop_phieu_nhap_result;
    stt_1 int := 1;
     stt_3 int := 1;
      stt_5 int := 1;
BEGIN
    DROP TABLE IF EXISTS tmp_bc_pn;
    CREATE TEMP TABLE tmp_bc_pn AS
    SELECT 
        0::float AS stt,
        5::float AS sys_order,
        1::float AS sys_print,
        0::float AS sys_total,
        pn.so_ct,
        pn.ngay_ct,
        ct.ma_kho,
        kho.ten_kho,
        pn.ma_ncc,
        ncc.ten_ncc,
        ct.ma_vt,
        vt.ten_vt,
        ct.ma_dvt,
        dvt.ten_dvt,
        ct.so_luong_nhap,
        ct.don_gia_nhap,
        (ct.so_luong_nhap * ct.don_gia_nhap) AS thanh_tien
    FROM phieu_nhap pn
    JOIN ct_phieu_nhap ct ON ct.so_ct = pn.so_ct AND ct.ma_ncc = pn.ma_ncc
    LEFT JOIN dmkho kho ON kho.ma_kho = ct.ma_kho
    LEFT JOIN dmncc ncc ON ncc.ma_ncc = pn.ma_ncc
    LEFT JOIN dmvt vt ON vt.ma_vt = ct.ma_vt
    LEFT JOIN dmdvt dvt ON dvt.ma_dvt = ct.ma_dvt
    WHERE 
        (ds_ma_kho = '' OR ct.ma_kho = ANY(string_to_array(ds_ma_kho, ',')::text[])) AND
        (ds_ma_vt = '' OR ct.ma_vt = ANY(string_to_array(ds_ma_vt, ',')::text[])) AND
        (pn.ngay_ct BETWEEN tu_ngay AND den_ngay) AND
        (
            search_keyword = '' OR
            lower(ct.ma_vt) LIKE '%' || search_keyword || '%' OR
            lower(vt.ten_vt) LIKE '%' || search_keyword || '%' OR
           lower(ct.ma_kho) LIKE '%' || search_keyword || '%' OR
           lower(kho.ten_kho) LIKE '%' || search_keyword || '%' OR
           lower(pn.ma_ncc) LIKE '%' || search_keyword || '%' OR
           lower(ncc.ten_ncc) LIKE '%' || search_keyword || '%' OR
           lower(ct.ma_dvt) LIKE '%' || search_keyword || '%' OR
           lower(dvt.ten_dvt) LIKE '%' || search_keyword || '%' OR 
            lower(pn.so_ct) LIKE '%' || search_keyword || '%'
        )
    ORDER BY pn.ngay_ct, pn.so_ct;


   -- Chèn thêm dữ liệu của dòng tổng
	insert into tmp_bc_pn(stt, sys_order, sys_print, sys_total, so_ct, ngay_ct, ma_kho, ten_kho, ma_ncc, ten_ncc, ma_vt, ten_vt, ma_dvt, ten_dvt, so_luong_nhap,don_gia_nhap, thanh_tien)
	select 
	0::float AS stt,
        3::float AS sys_order,
        1::float AS sys_print,
        1::float AS sys_total,
        MAX(a.so_ct) as so_ct,
        MAX(a.ngay_ct) as ngay_ct,
        'Chiết khấu'::text as ma_kho,
          null as ten_kho,
        MAX(a.ma_ncc) as ma_ncc,
       MAX( a.ten_ncc) as ten_ncc,
       'Thuế'::text as  ma_vt,
	null as ten_vt ,
        'Thanh toán'::text as ma_dvt,
        null as ten_dvt,
        SUM(so_luong_nhap) as so_luong_nhap,
        SUM(don_gia_nhap) as don_gia_nhap,
        SUM(thanh_tien) as thanh_tien
	from tmp_bc_pn a 
	where a.sys_order= 5
	group by a.so_ct ;
	insert into tmp_bc_pn(stt, sys_order, sys_print, sys_total, so_ct, ngay_ct, ma_kho, ten_kho, ma_ncc, ten_ncc, ma_vt, ten_vt, ma_dvt, ten_dvt, so_luong_nhap,don_gia_nhap, thanh_tien)
	select 
	0::float AS stt,
        1::float AS sys_order,
        1::float AS sys_print,
        1::float AS sys_total,
        'TỔNG CỘNG':: text as so_ct,
        MAX(a.ngay_ct) as ngay_ct,
        null as ma_kho,
        null as ten_kho,
        null as ma_ncc,
	null as ten_ncc,
	null as  ma_vt,
	null as ten_vt ,
        null as ma_dvt,
	null as ten_dvt,
        SUM(so_luong_nhap) as so_luong_nhap,
        SUM(don_gia_nhap) as don_gia_nhap,
        SUM(thanh_tien) as thanh_tien
	from tmp_bc_pn a 
	where a.sys_order= 3;
    Update tmp_bc_pn a set ten_kho = COALESCE(b.tong_chiet_khau::text, '0'),ten_vt = COALESCE(b.tong_thue::text, '0'),ten_dvt = COALESCE(b.tong_thanh_toan::text, '0')
    from phieu_nhap b
    where a.so_ct = b.so_ct and a.sys_order = 3;
    FOR r IN SELECT * FROM tmp_bc_pn order by so_ct desc, sys_order asc,  sys_total desc
    LOOP
    
         if r.sys_order = 5 then 
		r.stt := stt_1;
		stt_1:= stt_1 + 1;
		r.so_ct = null;
		r.ma_ncc = null;
		r.ten_ncc= null;  
           end if;
           if r.sys_order = 3 then 
           r.stt := stt_3;
		stt_3:= stt_3+1;
		stt_1:= 1;
           end if;
        RETURN NEXT r;
    END LOOP;
    RETURN;
END;
$$;


ALTER FUNCTION public.bc_tonghop_phieu_nhap(input_json json) OWNER TO postgres;

--
-- TOC entry 227 (class 1255 OID 66115)
-- Name: bc_tonghop_phieu_xuat(json); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION bc_tonghop_phieu_xuat(input_json json) RETURNS SETOF bc_tonghop_phieu_xuat_result
    LANGUAGE plpgsql
    AS $$
DECLARE
    ds_ma_kho text := input_json->>'ds_ma_kho';
    ds_ma_vt text := input_json->>'ds_ma_vt';
    search_keyword text := lower(trim(input_json->>'search_keyword'));
    tu_ngay timestamp := (input_json->>'tu_ngay')::timestamp;
    den_ngay timestamp := (input_json->>'den_ngay')::timestamp;
    r bc_tonghop_phieu_xuat_result;
    stt_1 int := 1;
     stt_3 int := 1;
      stt_5 int := 1;
BEGIN
    DROP TABLE IF EXISTS tmp_bc_pn;
    CREATE TEMP TABLE tmp_bc_pn AS
    SELECT 
        0::float AS stt,
        5::float AS sys_order,
        1::float AS sys_print,
        0::float AS sys_total,
        pn.so_ct,
        pn.ngay_ct,
        ct.ma_kho,
        kho.ten_kho,
        pn.ma_kh,
        kh.ten_kh,
        ct.ma_vt,
        vt.ten_vt,
        ct.ma_dvt,
        dvt.ten_dvt,
        ct.so_luong_xuat,
        ct.don_gia_xuat,
        (ct.so_luong_xuat * ct.don_gia_xuat) AS thanh_tien
    FROM phieu_xuat pn
    JOIN ct_phieu_xuat ct ON ct.so_ct = pn.so_ct AND ct.ma_kh = pn.ma_kh
    LEFT JOIN dmkho kho ON kho.ma_kho = ct.ma_kho
    LEFT JOIN dmkh kh ON kh.ma_kh = pn.ma_kh
    LEFT JOIN dmvt vt ON vt.ma_vt = ct.ma_vt
    LEFT JOIN dmdvt dvt ON dvt.ma_dvt = ct.ma_dvt
    WHERE 
        (ds_ma_kho = '' OR ct.ma_kho = ANY(string_to_array(ds_ma_kho, ',')::text[])) AND
        (ds_ma_vt = '' OR ct.ma_vt = ANY(string_to_array(ds_ma_vt, ',')::text[])) AND
        (pn.ngay_ct BETWEEN tu_ngay AND den_ngay) AND
        (
            search_keyword = '' OR
            lower(ct.ma_vt) LIKE '%' || search_keyword || '%' OR
            lower(vt.ten_vt) LIKE '%' || search_keyword || '%' OR
           lower(ct.ma_kho) LIKE '%' || search_keyword || '%' OR
           lower(kho.ten_kho) LIKE '%' || search_keyword || '%' OR
           lower(pn.ma_kh) LIKE '%' || search_keyword || '%' OR
           lower(kh.ten_kh) LIKE '%' || search_keyword || '%' OR
           lower(ct.ma_dvt) LIKE '%' || search_keyword || '%' OR
           lower(dvt.ten_dvt) LIKE '%' || search_keyword || '%' OR 
            lower(pn.so_ct) LIKE '%' || search_keyword || '%'
        )
    ORDER BY pn.ngay_ct, pn.so_ct;


   -- Chèn thêm dữ liệu của dòng tổng
	insert into tmp_bc_pn(stt, sys_order, sys_print, sys_total, so_ct, ngay_ct, ma_kho, ten_kho, ma_kh, ten_kh, ma_vt, ten_vt, ma_dvt, ten_dvt, so_luong_xuat,don_gia_xuat, thanh_tien)
	select 
	0::float AS stt,
        3::float AS sys_order,
        1::float AS sys_print,
        1::float AS sys_total,
        MAX(a.so_ct) as so_ct,
        MAX(a.ngay_ct) as ngay_ct,
        'Chiết khấu'::text as ma_kho,
          null as ten_kho,
        MAX(a.ma_kh) as ma_kh,
       MAX( a.ten_kh) as ten_kh,
       'Thuế'::text as  ma_vt,
	null as ten_vt ,
        'Thanh toán'::text as ma_dvt,
        null as ten_dvt,
        SUM(so_luong_xuat) as so_luong_xuat,
        SUM(don_gia_xuat) as don_gia_xuat,
        SUM(thanh_tien) as thanh_tien
	from tmp_bc_pn a 
	where a.sys_order= 5
	group by a.so_ct ;
	insert into tmp_bc_pn(stt, sys_order, sys_print, sys_total, so_ct, ngay_ct, ma_kho, ten_kho, ma_kh, ten_kh, ma_vt, ten_vt, ma_dvt, ten_dvt, so_luong_xuat,don_gia_xuat, thanh_tien)
	select 
	0::float AS stt,
        1::float AS sys_order,
        1::float AS sys_print,
        1::float AS sys_total,
        'TỔNG CỘNG':: text as so_ct,
        MAX(a.ngay_ct) as ngay_ct,
        null as ma_kho,
        null as ten_kho,
        null as ma_kh,
	null as ten_kh,
	null as  ma_vt,
	null as ten_vt ,
        null as ma_dvt,
	null as ten_dvt,
        SUM(so_luong_xuat) as so_luong_xuat,
        SUM(don_gia_xuat) as don_gia_xuat,
        SUM(thanh_tien) as thanh_tien
	from tmp_bc_pn a 
	where a.sys_order= 3;
    Update tmp_bc_pn a set ten_kho = COALESCE(b.tong_chiet_khau::text, '0'),ten_vt = COALESCE(b.tong_thue::text, '0'),ten_dvt = COALESCE(b.tong_thanh_toan::text, '0')
    from phieu_xuat b
    where a.so_ct = b.so_ct and a.sys_order = 3;
    FOR r IN SELECT * FROM tmp_bc_pn order by so_ct desc, sys_order asc,  sys_total desc
    LOOP
    
         if r.sys_order = 5 then 
		r.stt := stt_1;
		stt_1:= stt_1 + 1;
		r.so_ct = null;
		r.ma_kh = null;
		r.ten_kh= null;  
           end if;
           if r.sys_order = 3 then 
           r.stt := stt_3;
		stt_3:= stt_3+1;
		stt_1:= 1;
           end if;
        RETURN NEXT r;
    END LOOP;
    RETURN;
END;
$$;


ALTER FUNCTION public.bc_tonghop_phieu_xuat(input_json json) OWNER TO postgres;

--
-- TOC entry 225 (class 1255 OID 49389)
-- Name: bctonghopphieunhap(json); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION bctonghopphieunhap(input_json json) RETURNS SETOF bctonghopphieunhap_result
    LANGUAGE plpgsql
    AS $$
DECLARE
    r bctonghopphieunhap_result;
BEGIN
    drop table if exists tmp;
    create temp table tmp as
    SELECT 
	5::int as sys_order,
	1::int as sys_print,
	0:: int as sys_total,
        pn.so_ct,
        pn.ngay_ct,
        pn.ma_ncc,
        ncc.ten_ncc,
        ct.ma_vt,
        vt.ten_vt,
        ct.ma_dvt,
        dvt.ten_dvt, 
        ct.ma_kho,
        kho.ten_kho,
        ct.so_luong_nhap,
        ct.don_gia_nhap,
        (ct.so_luong_nhap * ct.don_gia_nhap),
        ct.ghi_chu
    FROM phieu_nhap pn
    INNER JOIN ct_phieu_nhap ct ON ct.so_ct = pn.so_ct AND ct.ma_ncc = pn.ma_ncc
    INNER JOIN dmncc ncc ON ncc.ma_ncc = pn.ma_ncc
    INNER JOIN dmkho kho ON kho.ma_kho = ct.ma_kho
    INNER JOIN dmvt vt ON vt.ma_vt = ct.ma_vt
    INNER JOIN dmdvt dvt ON dvt.ma_dvt = ct.ma_dvt
    WHERE 
        (input_json->>'ds_ma_vt' = '' OR ct.ma_vt = ANY(string_to_array(input_json->>'ds_ma_vt', ',')::text[])) AND
        (input_json->>'ds_ma_kho' = '' OR ct.ma_kho = ANY(string_to_array(input_json->>'ds_ma_kho', ',')::text[])) AND
        (input_json->>'ds_ma_ncc' = '' OR ct.ma_ncc = ANY(string_to_array(input_json->>'ds_ma_ncc', ',')::text[]))
    ORDER BY pn.ngay_ct DESC, pn.so_ct DESC;
 for r in select * from tmp
 loop
		return next r;
 end loop;
    
END;
$$;


ALTER FUNCTION public.bctonghopphieunhap(input_json json) OWNER TO postgres;

--
-- TOC entry 224 (class 1255 OID 41813)
-- Name: bctontheokho(json); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION bctontheokho(input_json json) RETURNS SETOF bctontheokho_result
    LANGUAGE plpgsql
    AS $$
DECLARE
    ds_ma_vt text;
    ds_ma_kho text;
    group_yn boolean;
    r bctontheokho_result;  -- Biến để lưu từng dòng kết quả
    stt_1 int;
    stt_5 int;
    stt_3 int;
BEGIN
    -- Lấy giá trị từ chuỗi JSON đầu vào
    ds_ma_vt := input_json->>'ds_ma_vt';
    ds_ma_kho := input_json->>'ds_ma_kho';
    group_yn := (input_json->>'group_yn')::boolean;

    -- Tạo một bảng tạm để tổng hợp các dữ liệu
    DROP TABLE IF EXISTS tmp;
    CREATE TEMP TABLE tmp AS
    SELECT 
        0::float AS stt,
        5::float AS sys_order,
        1::float AS sys_print,
        0::float AS sys_total,
        tk.ma_kho AS ma_kho,
        kho.ten_kho AS ten_kho,
        kho.mo_ta AS mo_ta_kho,
        kho.dia_chi AS dia_chi_kho,
        tk.ma_vt AS ma_vt,
        vt.ten_vt AS ten_vt,
        vt.rong AS rong,
        vt.cao AS cao,
        vt.khoi_luong AS khoi_luong,
        vt.mau_sac AS mau_sac,
        vt.kieu_dang AS kieu_dang,
        tk.ma_dvt AS ma_dvt,
        dvt.ten_dvt AS ten_dvt,
        dvt.mo_ta AS mo_ta_dvt,
        qd.ty_le_quy_doi AS ty_le_quy_doi,
        tk.so_luong_ton AS so_luong_ton,
        tk.ngay_cap_nhat AS ngay_cap_nhat
    FROM tonkho tk
    INNER JOIN dmkho kho ON kho.ma_kho = tk.ma_kho
    INNER JOIN dmvt vt ON vt.ma_vt = tk.ma_vt
    INNER JOIN dmdvt dvt ON dvt.ma_dvt = tk.ma_dvt
    LEFT JOIN dmqddvt qd ON qd.ma_vt = tk.ma_vt AND qd.ma_dvt = tk.ma_dvt
    WHERE 
        (ds_ma_vt = '' OR tk.ma_vt = ANY(string_to_array(ds_ma_vt, ',')::text[]))
        AND
        (ds_ma_kho = '' OR tk.ma_kho = ANY(string_to_array(ds_ma_kho, ',')::text[]))
    ORDER BY tk.ma_kho, tk.ma_vt, tk.ma_dvt;


    insert into tmp 
    
    SELECT 
    	   0::float AS stt,
		3::float AS sys_order,
           1::float AS sys_print,
           1::float AS sys_total,
           MAX(ma_kho) AS ma_kho,
           MAX(ten_kho) AS ten_kho,
           MAX (mo_ta_kho )AS mo_ta_kho,
           MAX(dia_chi_kho) AS dia_chi_kho,
           MAX(ma_vt) AS ma_vt,
           MAX(ten_vt) AS ten_vt,
          MAX( rong) AS rong,
          MAX( cao) AS cao,
          MAX( khoi_luong) AS khoi_luong,
           MAX(mau_sac) AS mau_sac,
           MAX(kieu_dang) AS kieu_dang,
         null AS ma_dvt,
           null   AS ten_dvt,
          null AS mo_ta_dvt,
          null AS ty_le_quy_doi,
           SUM(so_luong_ton) AS so_luong_ton,
          null AS ngay_cap_nhat
        from tmp
        group by ma_kho, ma_vt;
          insert into tmp 
    SELECT 
	   0::float AS stt,
    1::float AS sys_order,
           1::float AS sys_print,
           1::float AS sys_total,
           MAX(ma_kho) AS ma_kho,
           MAX(ten_kho) AS ten_kho,
           MAX (mo_ta_kho )AS mo_ta_kho,
           MAX(dia_chi_kho) AS dia_chi_kho,
           null AS ma_vt,
          null AS ten_vt,
       null AS rong,
         null AS cao,
          null AS khoi_luong,
          null AS mau_sac,
           null AS kieu_dang,
          null AS ma_dvt,
           null   AS ten_dvt,
           null AS mo_ta_dvt,
           null AS ty_le_quy_doi,
           SUM(so_luong_ton) AS so_luong_ton,
          null AS ngay_cap_nhat
        from tmp 
        group by ma_kho;

           insert into tmp 
    SELECT 
	   0::float AS stt,
	0::float AS sys_order,
           1::float AS sys_print,
           1::float AS sys_total,
           'Tổng cộng':: character varying AS ma_kho,
           null AS ten_kho,
           null AS mo_ta_kho,
        null  AS dia_chi_kho,
           null AS ma_vt,
          null AS ten_vt,
       null AS rong,
         null AS cao,
          null AS khoi_luong,
          null AS mau_sac,
           null AS kieu_dang,
          null AS ma_dvt,
           null   AS ten_dvt,
           null AS mo_ta_dvt,
           null AS ty_le_quy_doi,
           SUM(so_luong_ton) AS so_luong_ton,
          null AS ngay_cap_nhat
        from tmp;
    -- Truy vấn dữ liệu từ bảng tạm và trả về từng dòng
    stt_1 :=1;
    stt_5 :=1;
    stt_3 :=1;
    FOR r IN select * from tmp ORDER BY  ma_kho desc, ma_vt desc, ma_dvt desc, sys_total, sys_order
    LOOP
        -- Trả về từng dòng dữ liệu
        if r.sys_order = 5
        THEN
		r.stt = stt_5;
		stt_5 := stt_5+1;
		r.ma_kho = null;
		r.ten_kho =null;
		r.mo_ta_kho = null;
		r.dia_chi_kho = null;
		r.ma_vt = null;
		r.ten_vt = null;
		r.rong = null;
		r.cao = null;
		r.khoi_luong= null;
		r.mau_sac = null;
		r.kieu_dang = null;		
        END IF;
         if r.sys_order = 3
        THEN
		 r.stt = stt_3;
		stt_3 := stt_3 +1;
		stt_5 :=  1;
		r.ma_kho = null;
		r.ten_kho =null;
		r.mo_ta_kho = null;
		r.dia_chi_kho = null;
		 		
        END IF;
         if r.sys_order = 1
        THEN
		r.stt = stt_1;
		stt_1 := stt_1 +1;
		stt_3 :=  1;
		stt_5 :=  1;
		 
		 		
        END IF;
        RETURN NEXT r;
    END LOOP;

    -- Kết thúc hàm
    RETURN;
END;
$$;


ALTER FUNCTION public.bctontheokho(input_json json) OWNER TO postgres;

--
-- TOC entry 230 (class 1255 OID 90133)
-- Name: cbtontheokho(json); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION cbtontheokho(input_json json) RETURNS SETOF cbtontheokho_result
    LANGUAGE plpgsql
    AS $$
DECLARE
    ds_ma_vt text;
    ds_ma_kho text;
    group_yn boolean;
    r cbtontheokho_result;
    stt_1 int := 1;
    stt_5 int := 1;
    stt_3 int := 1;
BEGIN
    ds_ma_vt := input_json->>'ds_ma_vt';
    ds_ma_kho := input_json->>'ds_ma_kho';
    group_yn := (input_json->>'group_yn')::boolean;

    DROP TABLE IF EXISTS tmp;
    CREATE TEMP TABLE tmp AS
    SELECT 
        0::float AS stt,
        5::float AS sys_order,
        1::float AS sys_print,
        0::float AS sys_total,
        tk.ma_kho,
        kho.ten_kho,
        kho.mo_ta AS mo_ta_kho,
        kho.dia_chi AS dia_chi_kho,
        tk.ma_vt,
        vt.ten_vt,
        vt.rong,
        vt.cao,
        vt.khoi_luong,
        vt.mau_sac,
        vt.kieu_dang,
        tk.ma_dvt,
        dvt.ten_dvt,
        dvt.mo_ta AS mo_ta_dvt,
        qd.ty_le_quy_doi,
        tk.so_luong_ton,
        COALESCE(x.so_luong_dinh_xuat, 0) AS so_luong_dinh_xuat,
        COALESCE(n.so_luong_dang_nhap, 0) AS so_luong_dang_nhap,
        CASE
            WHEN (tk.so_luong_ton + COALESCE(n.so_luong_dang_nhap, 0)) < vt.min_ton THEN 'Dưới tồn tối thiểu'
            WHEN (tk.so_luong_ton + COALESCE(n.so_luong_dang_nhap, 0)) > vt.max_ton THEN 'Vượt tồn tối đa'
            ELSE NULL
        END AS canh_bao_ton_kho,
        tk.ngay_cap_nhat
    FROM tonkho tk
    INNER JOIN dmkho kho ON kho.ma_kho = tk.ma_kho
    INNER JOIN dmvt vt ON vt.ma_vt = tk.ma_vt
    INNER JOIN dmdvt dvt ON dvt.ma_dvt = tk.ma_dvt
    LEFT JOIN dmqddvt qd ON qd.ma_vt = tk.ma_vt AND qd.ma_dvt = tk.ma_dvt
    LEFT JOIN (
        SELECT ma_vt, ma_kho, ma_dvt, SUM(so_luong_xuat) AS so_luong_dinh_xuat
        FROM ct_phieu_xuat ct
        INNER JOIN phieu_xuat px ON px.so_ct = ct.so_ct AND px.ma_kh = ct.ma_kh
        WHERE px.trang_thai = 1
        GROUP BY ma_vt, ma_kho, ma_dvt
    ) x ON x.ma_vt = tk.ma_vt AND x.ma_kho = tk.ma_kho AND x.ma_dvt = tk.ma_dvt
    LEFT JOIN (
        SELECT ma_vt, ma_kho, ma_dvt, SUM(so_luong_nhap) AS so_luong_dang_nhap
        FROM ct_phieu_nhap ct
        INNER JOIN phieu_nhap pn ON pn.so_ct = ct.so_ct AND pn.ma_ncc = ct.ma_ncc
        WHERE pn.trang_thai = 1
        GROUP BY ma_vt, ma_kho, ma_dvt
    ) n ON n.ma_vt = tk.ma_vt AND n.ma_kho = tk.ma_kho AND n.ma_dvt = tk.ma_dvt
    WHERE 
        (ds_ma_vt = '' OR tk.ma_vt = ANY(string_to_array(ds_ma_vt, ',')::text[]))
        AND
        (ds_ma_kho = '' OR tk.ma_kho = ANY(string_to_array(ds_ma_kho, ',')::text[]))
    ORDER BY tk.ma_kho, tk.ma_vt, tk.ma_dvt;

    -- Gộp theo ma_kho, ma_vt
    INSERT INTO tmp
    SELECT 
        0, 3, 1, 1,
        MAX(ma_kho), MAX(ten_kho), MAX(mo_ta_kho), MAX(dia_chi_kho),
        MAX(ma_vt), MAX(ten_vt), MAX(rong), MAX(cao), MAX(khoi_luong),
        MAX(mau_sac), MAX(kieu_dang),
        NULL, NULL, NULL, NULL,
        SUM(so_luong_ton), NULL, NULL, NULL,
        NULL
    FROM tmp
    GROUP BY ma_kho, ma_vt;

    -- Gộp theo ma_kho
    INSERT INTO tmp
    SELECT 
        0, 1, 1, 1,
        MAX(ma_kho), MAX(ten_kho), MAX(mo_ta_kho), MAX(dia_chi_kho),
        NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL,
        SUM(so_luong_ton), NULL, NULL, NULL,
        NULL
    FROM tmp
    GROUP BY ma_kho;

    -- Tổng cộng toàn bộ
    INSERT INTO tmp
    SELECT 
        0, 0, 1, 1,
        'Tổng cộng', NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL,
        SUM(so_luong_ton), NULL, NULL, NULL,
        NULL
    FROM tmp;

    -- Trả về từng dòng
    FOR r IN
        SELECT * FROM tmp
        ORDER BY ma_kho DESC, ma_vt DESC, ma_dvt DESC, sys_total, sys_order
    LOOP
        IF r.sys_order = 5 THEN
            r.stt := stt_5;
            stt_5 := stt_5 + 1;
            r.ma_kho := NULL;
            r.ten_kho := NULL;
            r.mo_ta_kho := NULL;
            r.dia_chi_kho := NULL;
            r.ma_vt := NULL;
            r.ten_vt := NULL;
            r.rong := NULL;
            r.cao := NULL;
            r.khoi_luong := NULL;
            r.mau_sac := NULL;
            r.kieu_dang := NULL;
        ELSIF r.sys_order = 3 THEN
            r.stt := stt_3;
            stt_3 := stt_3 + 1;
            stt_5 := 1;
            r.ma_kho := NULL;
            r.ten_kho := NULL;
            r.mo_ta_kho := NULL;
            r.dia_chi_kho := NULL;
        ELSIF r.sys_order = 1 THEN
            r.stt := stt_1;
            stt_1 := stt_1 + 1;
            stt_3 := 1;
            stt_5 := 1;
        END IF;
        RETURN NEXT r;
    END LOOP;
    RETURN;
END;
$$;


ALTER FUNCTION public.cbtontheokho(input_json json) OWNER TO postgres;

--
-- TOC entry 215 (class 1255 OID 41057)
-- Name: delete_ct_phieu_nhap(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION delete_ct_phieu_nhap() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
  DELETE FROM ct_phieu_nhap
  WHERE so_ct = OLD.so_ct ;
  RETURN OLD;
END;
$$;


ALTER FUNCTION public.delete_ct_phieu_nhap() OWNER TO postgres;

--
-- TOC entry 222 (class 1255 OID 49181)
-- Name: delete_ct_phieu_xuat(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION delete_ct_phieu_xuat() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM ct_phieu_xuat
    WHERE so_ct = OLD.so_ct AND ma_kh = OLD.ma_kh;
    RETURN OLD;
END;
$$;


ALTER FUNCTION public.delete_ct_phieu_xuat() OWNER TO postgres;

--
-- TOC entry 223 (class 1255 OID 73782)
-- Name: delete_tonkho_when_dmkho_deleted(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION delete_tonkho_when_dmkho_deleted() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
  -- Xoá các bản ghi trong tonkho có ma_kho trùng với ma_kho vừa xoá
  DELETE FROM tonkho
  WHERE ma_kho = OLD.ma_kho;

  RETURN OLD;
END;
$$;


ALTER FUNCTION public.delete_tonkho_when_dmkho_deleted() OWNER TO postgres;

--
-- TOC entry 228 (class 1255 OID 73785)
-- Name: process_phieu_nhap(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION process_phieu_nhap(p_so_ct character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    rec RECORD;
BEGIN
  -- 1. Cập nhật trạng thái phiếu nhập
  UPDATE phieu_nhap
  SET trang_thai = 2
  WHERE so_ct = p_so_ct;

  -- 2. Duyệt từng dòng chi tiết phiếu nhập
  FOR rec IN
    SELECT ma_vt, ma_kho, so_luong_nhap, ma_dvt
    FROM ct_phieu_nhap
    WHERE so_ct = p_so_ct
  LOOP
    -- 2.1. Thử cập nhật tồn kho
    UPDATE tonkho
    SET 
      so_luong_ton = so_luong_ton + rec.so_luong_nhap,
      ngay_cap_nhat = NOW()
    WHERE 
      ma_vt = rec.ma_vt AND 
      ma_kho = rec.ma_kho AND 
      ma_dvt = rec.ma_dvt;

    -- 2.2. Nếu không cập nhật được dòng nào → thêm mới
    IF NOT FOUND THEN
      INSERT INTO tonkho (ma_vt, ma_kho, so_luong_ton, ma_dvt, ngay_cap_nhat)
      VALUES (rec.ma_vt, rec.ma_kho, rec.so_luong_nhap, rec.ma_dvt, NOW());
    END IF;
  END LOOP;
END;
$$;


ALTER FUNCTION public.process_phieu_nhap(p_so_ct character varying) OWNER TO postgres;

--
-- TOC entry 229 (class 1255 OID 73792)
-- Name: process_phieu_xuat(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION process_phieu_xuat(p_so_ct character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    rec RECORD;
BEGIN
  -- 1. Cập nhật trạng thái phiếu xuất
  UPDATE phieu_xuat
  SET trang_thai = 2
  WHERE so_ct = p_so_ct;

  -- 2. Duyệt từng dòng chi tiết phiếu xuất
  FOR rec IN
    SELECT ma_vt, ma_kho, so_luong_xuat, ma_dvt
    FROM ct_phieu_xuat
    WHERE so_ct = p_so_ct
  LOOP
    -- 2.1. Cập nhật tồn kho nếu đã có
    UPDATE tonkho
    SET 
      so_luong_ton = so_luong_ton - rec.so_luong_xuat,
      ngay_cap_nhat = NOW()
    WHERE 
      ma_vt = rec.ma_vt AND 
      ma_kho = rec.ma_kho AND 
      ma_dvt = rec.ma_dvt;

    -- 2.2. Nếu chưa có bản ghi tồn kho → tạo mới với số âm
    IF NOT FOUND THEN
      INSERT INTO tonkho (ma_vt, ma_kho, so_luong_ton, ma_dvt, ngay_cap_nhat)
      VALUES (rec.ma_vt, rec.ma_kho, -rec.so_luong_xuat, rec.ma_dvt, NOW());
    END IF;
  END LOOP;
END;
$$;


ALTER FUNCTION public.process_phieu_xuat(p_so_ct character varying) OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 206 (class 1259 OID 73765)
-- Name: audit_log; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE audit_log (
    id integer NOT NULL,
    table_name character varying(100) NOT NULL,
    operation character varying(10) NOT NULL,
    primary_key_data json,
    old_data json,
    new_data json,
    changed_by character varying(35),
    changed_at timestamp without time zone DEFAULT now()
);


ALTER TABLE public.audit_log OWNER TO postgres;

--
-- TOC entry 205 (class 1259 OID 73763)
-- Name: audit_log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE audit_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.audit_log_id_seq OWNER TO postgres;

--
-- TOC entry 2098 (class 0 OID 0)
-- Dependencies: 205
-- Name: audit_log_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE audit_log_id_seq OWNED BY audit_log.id;


--
-- TOC entry 195 (class 1259 OID 41019)
-- Name: ct_phieu_nhap; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE ct_phieu_nhap (
    ma_vt character varying(35) NOT NULL,
    ma_ncc character varying(35) NOT NULL,
    ma_kho character varying(35) NOT NULL,
    so_ct character varying(35) NOT NULL,
    so_luong_nhap integer,
    don_gia_nhap numeric(10,2),
    ma_dvt character varying(35) NOT NULL,
    ghi_chu text
);


ALTER TABLE public.ct_phieu_nhap OWNER TO postgres;

--
-- TOC entry 198 (class 1259 OID 41043)
-- Name: ct_phieu_xuat; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE ct_phieu_xuat (
    ma_vt character varying(35) NOT NULL,
    ma_kho character varying(35) NOT NULL,
    so_ct character varying(35) NOT NULL,
    so_luong_xuat integer,
    don_gia_xuat numeric(10,2),
    ma_kh character varying(35) NOT NULL,
    ma_dvt character varying(35) NOT NULL,
    ghi_chu text
);


ALTER TABLE public.ct_phieu_xuat OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 40977)
-- Name: dmdvt; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmdvt (
    ma_dvt character varying(35) NOT NULL,
    ten_dvt character varying(100),
    mo_ta text,
    trangthai integer
);


ALTER TABLE public.dmdvt OWNER TO postgres;

--
-- TOC entry 196 (class 1259 OID 41027)
-- Name: dmkh; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmkh (
    ma_kh character varying(35) NOT NULL,
    ten_kh character varying(100),
    dia_chi text,
    dien_thoai character(12),
    mo_ta text,
    ma_so_thue text
);


ALTER TABLE public.dmkh OWNER TO postgres;

--
-- TOC entry 192 (class 1259 OID 40998)
-- Name: dmkho; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmkho (
    ma_kho character varying(35) NOT NULL,
    ten_kho character varying(100),
    mo_ta text,
    dia_chi text
);


ALTER TABLE public.dmkho OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 40990)
-- Name: dmncc; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmncc (
    ma_ncc character varying(35) NOT NULL,
    ten_ncc text,
    dia_chi text,
    ghi_chu text,
    dien_thoai character varying(11),
    email character varying(50),
    ma_so_thue text
);


ALTER TABLE public.dmncc OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 41076)
-- Name: dmqddvt; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmqddvt (
    ma_vt character varying(35) NOT NULL,
    ma_dvt character varying(35) NOT NULL,
    ty_le_quy_doi numeric(5,2)
);


ALTER TABLE public.dmqddvt OWNER TO postgres;

--
-- TOC entry 190 (class 1259 OID 40985)
-- Name: dmvt; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE dmvt (
    ma_vt character varying(35) NOT NULL,
    ma_loai_vt character varying(35) NOT NULL,
    ten_vt character varying(100),
    min_ton integer,
    max_ton integer,
    barcode character varying(50),
    url character varying(100),
    rong numeric(5,2),
    cao numeric(5,2),
    khoi_luong numeric(5,2),
    mau_sac character varying,
    kieu_dang character varying,
    trangthai integer,
    mo_ta text,
    ma_dvt character varying(35)
);


ALTER TABLE public.dmvt OWNER TO postgres;

--
-- TOC entry 188 (class 1259 OID 40969)
-- Name: loai_vat_tu; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE loai_vat_tu (
    ma_loai_vt character varying(35) NOT NULL,
    ten_loai_vt character varying(100),
    mo_ta text,
    trangthai integer
);


ALTER TABLE public.loai_vat_tu OWNER TO postgres;

--
-- TOC entry 194 (class 1259 OID 41011)
-- Name: phieu_nhap; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE phieu_nhap (
    so_ct character varying(35) NOT NULL,
    ma_ncc character varying(35) NOT NULL,
    ngay_ct timestamp without time zone,
    dien_giai text,
    trang_thai integer,
    bien_so_xe text,
    ngay_van_chuyen timestamp without time zone,
    tt_thanhtoan integer,
    chietkhau integer,
    ma_so_thue text,
    thue integer,
    tong_thanh_toan numeric(18,2),
    tong_chiet_khau numeric(18,2),
    tong_thue numeric(18,2)
);


ALTER TABLE public.phieu_nhap OWNER TO postgres;

--
-- TOC entry 197 (class 1259 OID 41035)
-- Name: phieu_xuat; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE phieu_xuat (
    so_ct character varying(35) NOT NULL,
    ngay_ct timestamp without time zone,
    dien_giai text,
    ma_kh character varying(35) NOT NULL,
    trang_thai integer,
    bien_so_xe text,
    ngay_van_chuyen timestamp without time zone,
    tt_thanhtoan integer,
    chietkhau integer,
    ma_so_thue text,
    thue integer,
    tong_thanh_toan numeric(18,2),
    tong_chiet_khau numeric(18,2),
    tong_thue numeric(18,2),
    nguoi_giao character varying(100),
    dia_diem_giao text
);


ALTER TABLE public.phieu_xuat OWNER TO postgres;

--
-- TOC entry 193 (class 1259 OID 41006)
-- Name: tonkho; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE tonkho (
    ma_vt character varying(35) NOT NULL,
    ma_kho character varying(35) NOT NULL,
    so_luong_ton integer,
    ma_dvt character varying(35) NOT NULL,
    ngay_cap_nhat timestamp without time zone
);


ALTER TABLE public.tonkho OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 41051)
-- Name: userinfo; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE userinfo (
    id character varying(35) NOT NULL,
    username character varying(35),
    fullname character varying(100),
    password text,
    role integer,
    email character varying(50),
    trangthai integer
);


ALTER TABLE public.userinfo OWNER TO postgres;

--
-- TOC entry 1935 (class 2604 OID 73768)
-- Name: id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY audit_log ALTER COLUMN id SET DEFAULT nextval('audit_log_id_seq'::regclass);


--
-- TOC entry 2089 (class 0 OID 73765)
-- Dependencies: 206
-- Data for Name: audit_log; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY audit_log (id, table_name, operation, primary_key_data, old_data, new_data, changed_by, changed_at) FROM stdin;
195	dmkho	INSERT	{\r\n  "ma_kho": "KHO01"\r\n}	\N	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Kho nguyên vật liệu sản xuất đúc",\r\n  "dia_chi": "Thái Bình"\r\n}	15	2025-05-27 09:54:04.179243
196	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 21554,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:10.736Z"\r\n}	15	2025-05-27 09:54:04.198953
197	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_dvt": "DVT06"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:14.168Z"\r\n}	15	2025-05-27 09:54:04.208977
198	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 23,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:18.032Z"\r\n}	15	2025-05-27 09:54:04.217159
199	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT05",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:23.032Z"\r\n}	15	2025-05-27 09:54:04.224425
200	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT08",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "VT08",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:26.705Z"\r\n}	15	2025-05-27 09:54:04.233186
201	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:30.32Z"\r\n}	15	2025-05-27 09:54:04.240185
202	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2355,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:34.225Z"\r\n}	15	2025-05-27 09:54:04.248206
232	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 11:03:10.947747
234	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817Z"\r\n}	15	2025-05-27 11:03:11.906906
259	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT02"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_xuat": 12,\r\n  "don_gia_xuat": 34235.00,\r\n  "ghi_chu": "523"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_xuat": 12,\r\n  "don_gia_xuat": 34235.0,\r\n  "ghi_chu": "523"\r\n}	15	2025-05-27 12:04:32.867557
287	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:06:36.810814
312	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 432,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "sd"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 432,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "sd"\r\n}	15	2025-05-28 00:06:01.530006
318	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO06",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 34.00,\r\n  "ghi_chu": "sd"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 34.0,\r\n  "ghi_chu": "sd"\r\n}	15	2025-05-28 00:06:01.577818
320	userinfo	INSERT	{\r\n  "id": "16"\r\n}	\N	{\r\n  "id": "16",\r\n  "username": "test",\r\n  "fullname": "236236",\r\n  "password": "23523623",\r\n  "email": "23623@gmail.com",\r\n  "role": 1,\r\n  "trangthai": 1\r\n}	15	2025-05-28 00:06:33.827957
341	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 125142,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:36.277Z"\r\n}	15	2025-05-28 07:36:21.063087
344	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 1254,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:52.117Z"\r\n}	15	2025-05-28 07:36:21.090328
203	dmkho	UPDATE	{\r\n  "ma_kho": "KHO01"\r\n}	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Kho nguyên vật liệu sản xuất đúc",\r\n  "dia_chi": "Thái Bình"\r\n}	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Kho nguyên vật liệu sản xuất đúc",\r\n  "dia_chi": "Thái Bình"\r\n}	15	2025-05-27 10:18:22.167156
205	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:14.168"\r\n}	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:14.168"\r\n}	15	2025-05-27 10:18:23.118222
207	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT05",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:23.032"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:23.032"\r\n}	15	2025-05-27 10:18:23.132258
209	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:30.32"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:30.32"\r\n}	15	2025-05-27 10:18:23.147095
233	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297Z"\r\n}	15	2025-05-27 11:03:11.897885
260	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 0,\r\n  "don_gia_xuat": 35.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 12:04:32.875426
288	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.00,\r\n  "ghi_chu": null\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 23:06:36.82197
314	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.00,\r\n  "ghi_chu": "23"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.0,\r\n  "ghi_chu": "23"\r\n}	15	2025-05-28 00:06:01.546125
347	phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-280525074408"\r\n}	\N	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_ncc": "4",\r\n  "ngay_ct": "2025-05-28T00:44:08.829Z",\r\n  "dien_giai": "TEST 1",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "12-MD543",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00Z",\r\n  "tt_thanhtoan": 1,\r\n  "chietkhau": 1,\r\n  "ma_so_thue": "9583564964",\r\n  "thue": 1,\r\n  "tong_thanh_toan": 94433095.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-28 07:45:19.635709
352	userinfo	INSERT	{\r\n  "id": "17"\r\n}	\N	{\r\n  "id": "17",\r\n  "username": "TEST1123",\r\n  "fullname": "235",\r\n  "password": "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",\r\n  "email": null,\r\n  "role": 1,\r\n  "trangthai": 1\r\n}	15	2025-05-28 12:27:08.918286
204	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 21554,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:10.736"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 21554,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:10.736"\r\n}	15	2025-05-27 10:18:23.108205
206	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 23,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:18.032"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 23,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:18.032"\r\n}	15	2025-05-27 10:18:23.125259
208	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2355,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:34.225"\r\n}	15	2025-05-27 10:18:23.139582
236	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 11:08:37.538388
237	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 243,\r\n  "ngay_cap_nhat": "2025-05-27T04:03:09.729"\r\n}	15	2025-05-27 11:08:38.4642
238	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297"\r\n}	15	2025-05-27 11:08:38.475236
239	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817"\r\n}	15	2025-05-27 11:08:38.48619
240	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:08:36.202Z"\r\n}	15	2025-05-27 11:08:38.497243
261	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926"\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.50,\r\n  "tong_chiet_khau": 3313.50,\r\n  "tong_thue": 0.00\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.5,\r\n  "tong_chiet_khau": 3313.5,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-27 22:29:28.262817
266	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:30:50.952908
289	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937"\r\n}	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-10T14:39:37.456",\r\n  "dien_giai": "ets",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 2,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "sadgsadg",\r\n  "thue": 2,\r\n  "tong_thanh_toan": 608136.48,\r\n  "tong_chiet_khau": 24132.40,\r\n  "tong_thue": 28958.88\r\n}	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-10T14:39:37.456",\r\n  "dien_giai": "ets",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 2,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "sadgsadg",\r\n  "thue": 2,\r\n  "tong_thanh_toan": 608136.48,\r\n  "tong_chiet_khau": 24132.4,\r\n  "tong_thue": 28958.88\r\n}	15	2025-05-27 23:08:45.95115
293	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO06",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 342,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 342,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:08:47.405033
299	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO05",\r\n  "ma_dvt": "DVT05"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT05",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "234"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT05",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "234"\r\n}	15	2025-05-27 23:08:47.450753
315	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO07",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO07",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "dgsdg"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO07",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "dgsdg"\r\n}	15	2025-05-28 00:06:01.554124
348	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 23554,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "TEST"\r\n}	15	2025-05-28 07:45:19.678727
353	userinfo	INSERT	{\r\n  "id": "18"\r\n}	\N	{\r\n  "id": "18",\r\n  "username": "PHUCTEST1",\r\n  "fullname": "PHUCTEST1",\r\n  "password": "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",\r\n  "email": "mphucoca@gmail.com",\r\n  "role": 0,\r\n  "trangthai": 1\r\n}	17	2025-05-28 12:30:21.032071
210	dmkho	UPDATE	{\r\n  "ma_kho": "KHO01"\r\n}	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Kho nguyên vật liệu sản xuất đúc",\r\n  "dia_chi": "Thái Bình"\r\n}	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Kho nguyên vật liệu sản xuất đúc",\r\n  "dia_chi": "Thái Bình"\r\n}	15	2025-05-27 10:25:19.259598
214	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT05",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:23.032"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:23.032"\r\n}	15	2025-05-27 10:25:20.200074
219	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT02",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT02",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:02.465Z"\r\n}	15	2025-05-27 10:26:11.053909
241	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 11:11:46.42978
242	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 243,\r\n  "ngay_cap_nhat": "2025-05-27T04:03:09.729"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 243,\r\n  "ngay_cap_nhat": "2025-05-27T04:03:09.729"\r\n}	15	2025-05-27 11:11:48.799242
243	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297"\r\n}	15	2025-05-27 11:11:48.80972
244	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 5446,\r\n  "ngay_cap_nhat": "2025-05-27T04:11:45.168Z"\r\n}	15	2025-05-27 11:11:48.811761
245	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:08:36.202Z"\r\n}	15	2025-05-27 11:11:48.831077
246	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT02",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT02",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 3532,\r\n  "ngay_cap_nhat": "2025-05-27T04:11:39.416Z"\r\n}	15	2025-05-27 11:11:48.84058
262	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:29:29.546848
263	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-250525163507"\r\n}	{\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:35:07.798",\r\n  "dien_giai": "test khởi tạo và lưu log",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "kfjasdfkjkl",\r\n  "ngay_van_chuyen": "2025-05-02T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 9778730.88,\r\n  "tong_chiet_khau": 407447.12,\r\n  "tong_thue": 0.00\r\n}	{\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:35:07.798",\r\n  "dien_giai": "test khởi tạo và lưu log",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "kfjasdfkjkl",\r\n  "ngay_van_chuyen": "2025-05-02T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 9778730.88,\r\n  "tong_chiet_khau": 407447.12,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-27 22:30:50.007161
265	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO03",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO03",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 243,\r\n  "don_gia_nhap": 4235.00,\r\n  "ghi_chu": "5235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO03",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 243,\r\n  "don_gia_nhap": 4235.0,\r\n  "ghi_chu": "5235"\r\n}	15	2025-05-27 22:30:50.944134
290	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "sdg"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "sdg"\r\n}	15	2025-05-27 23:08:47.37816
296	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:08:47.429336
301	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:09:56.33435
322	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": null,\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": null,\r\n  "tong_chiet_khau": null,\r\n  "tong_thue": null,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": "1",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 57013.82,\r\n  "tong_chiet_khau": 3639.18,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	16	2025-05-28 00:15:50.828741
211	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 21554,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:10.736"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 21554,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:10.736"\r\n}	15	2025-05-27 10:25:20.183039
217	dmkho	INSERT	{\r\n  "ma_kho": "KHO02"\r\n}	\N	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 10:26:11.024048
247	dmdvt	INSERT	{\r\n  "ma_dvt": "dg"\r\n}	\N	{\r\n  "ma_dvt": "dg",\r\n  "ten_dvt": "236",\r\n  "mo_ta": "236",\r\n  "trangthai": 1\r\n}	15	2025-05-27 11:32:05.598658
264	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT07"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT07",\r\n  "so_luong_nhap": 214,\r\n  "don_gia_nhap": 42532.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-250525163507",\r\n  "ma_dvt": "DVT07",\r\n  "so_luong_nhap": 214,\r\n  "don_gia_nhap": 42532.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:30:50.938089
291	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 43,\r\n  "don_gia_nhap": 34.00,\r\n  "ghi_chu": "234"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 43,\r\n  "don_gia_nhap": 34.0,\r\n  "ghi_chu": "234"\r\n}	15	2025-05-27 23:08:47.385313
297	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO05",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.00,\r\n  "ghi_chu": "325"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.0,\r\n  "ghi_chu": "325"\r\n}	15	2025-05-27 23:08:47.438262
300	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659"\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.40,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 625215359307.40\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.4,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 625215359307.4\r\n}	15	2025-05-27 23:09:55.374231
323	ct_phieu_xuat	INSERT	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 235,\r\n  "don_gia_xuat": 235.0,\r\n  "ghi_chu": "236"\r\n}	16	2025-05-28 00:15:53.225096
326	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 235,\r\n  "don_gia_xuat": 235.00,\r\n  "ghi_chu": "236"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 235,\r\n  "don_gia_xuat": 235.0,\r\n  "ghi_chu": "236"\r\n}	15	2025-05-28 00:17:32.513696
349	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_nhap": 35235,\r\n  "don_gia_nhap": 2523.0,\r\n  "ghi_chu": "TEST2"\r\n}	15	2025-05-28 07:45:19.696952
354	userinfo	INSERT	{\r\n  "id": "19"\r\n}	\N	{\r\n  "id": "19",\r\n  "username": "PHUCTEST2",\r\n  "fullname": "PHUCTEST2",\r\n  "password": "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",\r\n  "email": "mphucoca@gmail.com",\r\n  "role": 1,\r\n  "trangthai": 1\r\n}	17	2025-05-28 12:30:41.771366
212	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:14.168"\r\n}	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:14.168"\r\n}	15	2025-05-27 10:25:20.189034
248	dmdvt	UPDATE	{\r\n  "ma_dvt": "dg"\r\n}	{\r\n  "ma_dvt": "dg",\r\n  "ten_dvt": "236",\r\n  "mo_ta": "236",\r\n  "trangthai": 1\r\n}	{\r\n  "ma_dvt": "dg",\r\n  "ten_dvt": "236",\r\n  "mo_ta": "23643",\r\n  "trangthai": 1\r\n}	15	2025-05-27 11:32:08.515213
267	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525140242"\r\n}	{\r\n  "so_ct": "PX000001-110525140242",\r\n  "ngay_ct": "2025-05-11T07:02:42.865",\r\n  "dien_giai": "dsgsdg",\r\n  "ma_kh": null,\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": null,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 5,\r\n  "tong_thanh_toan": null,\r\n  "tong_chiet_khau": null,\r\n  "tong_thue": null,\r\n  "nguoi_giao": null,\r\n  "dia_diem_giao": null\r\n}	{\r\n  "so_ct": "PX000001-110525140242",\r\n  "ngay_ct": "2025-05-11T07:02:42.865",\r\n  "dien_giai": "dsgsdg",\r\n  "ma_kh": "KH03",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": null,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": null,\r\n  "thue": 5,\r\n  "tong_thanh_toan": 0.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": null,\r\n  "dia_diem_giao": null\r\n}	15	2025-05-27 22:33:38.961259
268	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525140242"\r\n}	{\r\n  "so_ct": "PX000001-110525140242",\r\n  "ngay_ct": "2025-05-11T07:02:42.865",\r\n  "dien_giai": "dsgsdg",\r\n  "ma_kh": null,\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": null,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": null,\r\n  "thue": 5,\r\n  "tong_thanh_toan": 0.00,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": null,\r\n  "dia_diem_giao": null\r\n}	{\r\n  "so_ct": "PX000001-110525140242",\r\n  "ngay_ct": "2025-05-11T07:02:42.865",\r\n  "dien_giai": "dsgsdg",\r\n  "ma_kh": "KH08",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": null,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": null,\r\n  "thue": 5,\r\n  "tong_thanh_toan": 0.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": null,\r\n  "dia_diem_giao": null\r\n}	15	2025-05-27 22:34:00.590927
292	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 432,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "sd"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 432,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "sd"\r\n}	15	2025-05-27 23:08:47.395814
298	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO06",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 34.00,\r\n  "ghi_chu": "sd"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 34.0,\r\n  "ghi_chu": "sd"\r\n}	15	2025-05-27 23:08:47.446918
324	ct_phieu_xuat	INSERT	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 23,\r\n  "don_gia_xuat": 236.0,\r\n  "ghi_chu": "235"\r\n}	16	2025-05-28 00:15:53.241023
328	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": null,\r\n  "trang_thai": 3,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 57013.82,\r\n  "tong_chiet_khau": 3639.18,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": "1",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 57013.82,\r\n  "tong_chiet_khau": 3639.18,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	15	2025-05-28 00:17:40.917996
350	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525165947"\r\n}	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:59:46.99",\r\n  "dien_giai": "ádfjhkjhsdf 343",\r\n  "trang_thai": 3,\r\n  "bien_so_xe": "235",\r\n  "ngay_van_chuyen": "2025-05-22T17:00:00",\r\n  "tt_thanhtoan": 6,\r\n  "chietkhau": 3,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 5,\r\n  "tong_thanh_toan": 56826.53,\r\n  "tong_chiet_khau": 1104.50,\r\n  "tong_thue": 2706.03\r\n}	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:59:46.99",\r\n  "dien_giai": "ádfjhkjhsdf 343",\r\n  "trang_thai": 3,\r\n  "bien_so_xe": "235",\r\n  "ngay_van_chuyen": "2025-05-22T17:00:00",\r\n  "tt_thanhtoan": 6,\r\n  "chietkhau": 3,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 5,\r\n  "tong_thanh_toan": 56826.525,\r\n  "tong_chiet_khau": 1104.5,\r\n  "tong_thue": 2706.025\r\n}	15	2025-05-28 10:07:32.276929
351	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT07"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_dvt": "DVT07",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_dvt": "DVT07",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 10:07:33.403486
355	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-280525074408"\r\n}	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_ncc": "4",\r\n  "ngay_ct": "2025-05-28T00:44:08.829",\r\n  "dien_giai": "TEST 1",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "12-MD543",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 1,\r\n  "chietkhau": 1,\r\n  "ma_so_thue": "9583564964",\r\n  "thue": 1,\r\n  "tong_thanh_toan": 94433095.00,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 0.00\r\n}	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_ncc": "4",\r\n  "ngay_ct": "2025-05-28T00:44:08.829",\r\n  "dien_giai": "TEST 1",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "12-MD543",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 1,\r\n  "chietkhau": 1,\r\n  "ma_so_thue": "9583564964",\r\n  "thue": 1,\r\n  "tong_thanh_toan": 94433095.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0\r\n}	19	2025-05-28 15:10:03.586109
356	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 23554,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "TEST"\r\n}	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 23554,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "TEST"\r\n}	19	2025-05-28 15:10:05.365825
213	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 23,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:18.032"\r\n}	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 23,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:18.032"\r\n}	15	2025-05-27 10:25:20.195071
218	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_dvt": "DVT06"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 325,\r\n  "ngay_cap_nhat": "2025-05-27T03:25:58.593Z"\r\n}	15	2025-05-27 10:26:11.045323
249	dmkh	UPDATE	{\r\n  "ma_kh": "KH01"\r\n}	{\r\n  "ma_kh": "KH01",\r\n  "ten_kh": "Công ty A",\r\n  "dia_chi": "Hà Nội",\r\n  "dien_thoai": "0901234567  ",\r\n  "mo_ta": "Khách hàng miền Bắc",\r\n  "ma_so_thue": null\r\n}	{\r\n  "ma_kh": "KH01",\r\n  "ten_kh": "Công ty A",\r\n  "dia_chi": "Hà Nội",\r\n  "dien_thoai": "09012345673",\r\n  "mo_ta": "Khách hàng miền Bắc",\r\n  "ma_so_thue": null\r\n}	15	2025-05-27 11:41:31.334778
250	dmkh	UPDATE	{\r\n  "ma_kh": "KH05"\r\n}	{\r\n  "ma_kh": "KH05",\r\n  "ten_kh": "Công ty E",\r\n  "dia_chi": "Cần Thơ",\r\n  "dien_thoai": "0905678901  ",\r\n  "mo_ta": "Khách hàng nông nghiệp",\r\n  "ma_so_thue": null\r\n}	{\r\n  "ma_kh": "KH05",\r\n  "ten_kh": "Công ty E",\r\n  "dia_chi": "Cần Thơ",\r\n  "dien_thoai": "0905678901u",\r\n  "mo_ta": "Khách hàng nông nghiệp",\r\n  "ma_so_thue": null\r\n}	15	2025-05-27 11:41:54.548196
269	ct_phieu_xuat	INSERT	{\r\n  "so_ct": "PX000001-110525140242",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PX000001-110525140242",\r\n  "ma_kh": "KH08",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 0,\r\n  "don_gia_xuat": 354.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:34:01.504002
274	ct_phieu_xuat	INSERT	{\r\n  "so_ct": "PX000015-270525223438",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PX000015-270525223438",\r\n  "ma_kh": "4",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 0,\r\n  "don_gia_xuat": 235236.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:35:02.55564
294	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.00,\r\n  "ghi_chu": "23"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.0,\r\n  "ghi_chu": "23"\r\n}	15	2025-05-27 23:08:47.413057
325	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": null,\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 57013.82,\r\n  "tong_chiet_khau": 3639.18,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ngay_ct": "2025-05-11T07:14:56.889",\r\n  "dien_giai": "ádgsdg",\r\n  "ma_kh": "1",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "3532sdf",\r\n  "ngay_van_chuyen": "2025-05-15T17:00:00",\r\n  "tt_thanhtoan": 4,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "25235sđf",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 57013.82,\r\n  "tong_chiet_khau": 3639.18,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "dsgadsg",\r\n  "dia_diem_giao": "sdgsdg"\r\n}	15	2025-05-28 00:17:31.8461
330	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 23,\r\n  "don_gia_xuat": 236.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 23,\r\n  "don_gia_xuat": 236.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 00:17:41.516875
357	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02"\r\n}	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_nhap": 35235,\r\n  "don_gia_nhap": 2523.00,\r\n  "ghi_chu": "TEST2"\r\n}	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_ncc": "4",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PN000015-280525074408",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_nhap": 35235,\r\n  "don_gia_nhap": 2523.0,\r\n  "ghi_chu": "TEST2"\r\n}	19	2025-05-28 15:10:05.365825
215	ton_kho	UPDATE	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2355,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:34.225"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2355,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:34.225"\r\n}	15	2025-05-27 10:25:20.205475
220	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT08",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT08",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:05.969Z"\r\n}	15	2025-05-27 10:26:11.059906
251	dmncc	UPDATE	{\r\n  "ma_ncc": "3"\r\n}	{\r\n  "ma_ncc": "3",\r\n  "ten_ncc": "FSDG2",\r\n  "dia_chi": "Hưng Yên",\r\n  "ghi_chu": "23326",\r\n  "dien_thoai": "095485495",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "23636235"\r\n}	{\r\n  "ma_ncc": "3",\r\n  "ten_ncc": "FSDG2",\r\n  "dia_chi": "Hưng Yên s",\r\n  "ghi_chu": "23326",\r\n  "dien_thoai": "095485495",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "23636235"\r\n}	15	2025-05-27 11:45:09.57145
270	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927"\r\n}	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ngay_ct": "2025-05-25T06:59:27.806",\r\n  "dien_giai": "236456",\r\n  "ma_kh": null,\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "2362362",\r\n  "ngay_van_chuyen": "2025-05-14T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "23563253",\r\n  "thue": 0,\r\n  "tong_thanh_toan": 410820.00,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": "2362363",\r\n  "dia_diem_giao": "246236236"\r\n}	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ngay_ct": "2025-05-25T06:59:27.806",\r\n  "dien_giai": "236456",\r\n  "ma_kh": "KH03",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "2362362",\r\n  "ngay_van_chuyen": "2025-05-14T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "235235",\r\n  "thue": 0,\r\n  "tong_thanh_toan": 410820.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "2362363",\r\n  "dia_diem_giao": "246236236"\r\n}	15	2025-05-27 22:34:30.014319
272	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 0,\r\n  "don_gia_xuat": 35.00,\r\n  "ghi_chu": null\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 0,\r\n  "don_gia_xuat": 35.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 22:34:30.887186
295	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO07",\r\n  "ma_dvt": "DVT06"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO07",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "dgsdg"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO07",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "dgsdg"\r\n}	15	2025-05-27 23:08:47.422335
302	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.00,\r\n  "ghi_chu": null\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 23:09:56.340854
327	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT04"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 23,\r\n  "don_gia_xuat": 236.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_xuat": 23,\r\n  "don_gia_xuat": 236.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 00:17:32.520748
329	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 235,\r\n  "don_gia_xuat": 235.00,\r\n  "ghi_chu": "236"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PX000001-110525141456",\r\n  "ma_kh": "1",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_xuat": 235,\r\n  "don_gia_xuat": 235.0,\r\n  "ghi_chu": "236"\r\n}	15	2025-05-28 00:17:41.510366
358	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000015-270525223438"\r\n}	{\r\n  "so_ct": "PX000015-270525223438",\r\n  "ngay_ct": "2025-05-27T15:34:38.911",\r\n  "dien_giai": "ưetwet",\r\n  "ma_kh": null,\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "ử",\r\n  "ngay_van_chuyen": "2025-05-08T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": "32623623",\r\n  "thue": 7,\r\n  "tong_thanh_toan": 0.00,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": "rew",\r\n  "dia_diem_giao": "ưetwet"\r\n}	{\r\n  "so_ct": "PX000015-270525223438",\r\n  "ngay_ct": "2025-05-27T15:34:38.911",\r\n  "dien_giai": "ưetwet",\r\n  "ma_kh": "4",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "ử",\r\n  "ngay_van_chuyen": "2025-05-08T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": "32623623",\r\n  "thue": 7,\r\n  "tong_thanh_toan": 60353324.34,\r\n  "tong_chiet_khau": 1623128.4,\r\n  "tong_thue": 7872172.74,\r\n  "nguoi_giao": "rew",\r\n  "dia_diem_giao": "ưetwet"\r\n}	18	2025-05-28 15:50:43.74116
359	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000015-270525223438",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PX000015-270525223438",\r\n  "ma_kh": "4",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_xuat": 230,\r\n  "don_gia_xuat": 235236.0,\r\n  "ghi_chu": "235"\r\n}	18	2025-05-28 15:50:47.11817
361	ct_phieu_xuat	INSERT	{\r\n  "so_ct": "PX000018-280525155109",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "so_ct": "PX000018-280525155109",\r\n  "ma_kh": "4",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_xuat": 232,\r\n  "don_gia_xuat": 32.0,\r\n  "ghi_chu": null\r\n}	18	2025-05-28 15:51:29.151202
216	ton_kho	INSERT	{\r\n  "ma_kho": "KHO01",\r\n  "ma_vt": "VT07",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 34,\r\n  "ngay_cap_nhat": "2025-05-27T02:53:30.32"\r\n}	15	2025-05-27 10:25:20.210481
221	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT10",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT10",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2356,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:09.977Z"\r\n}	15	2025-05-27 10:26:11.065926
252	dmncc	UPDATE	{\r\n  "ma_ncc": "1"\r\n}	{\r\n  "ma_ncc": "1",\r\n  "ten_ncc": "Tên nhà cung cấp test",\r\n  "dia_chi": "Thái Bình Hưng Yên",\r\n  "ghi_chu": "Note thử test sửa",\r\n  "dien_thoai": "0384653035",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "123125234235"\r\n}	{\r\n  "ma_ncc": "1",\r\n  "ten_ncc": "Tên nhà cung cấp test",\r\n  "dia_chi": "Thái Bình Hưng Yên s",\r\n  "ghi_chu": "Note thử test sửa",\r\n  "dien_thoai": "0384653035",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "123125234235"\r\n}	15	2025-05-27 11:45:50.09225
271	ct_phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT02"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_xuat": 12,\r\n  "don_gia_xuat": 34235.00,\r\n  "ghi_chu": "523"\r\n}	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PX000001-250525135927",\r\n  "ma_kh": "2",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_xuat": 12,\r\n  "don_gia_xuat": 34235.0,\r\n  "ghi_chu": "523"\r\n}	15	2025-05-27 22:34:30.879146
273	phieu_xuat	INSERT	{\r\n  "so_ct": "PX000015-270525223438"\r\n}	\N	{\r\n  "so_ct": "PX000015-270525223438",\r\n  "ngay_ct": "2025-05-27T15:34:38.911Z",\r\n  "dien_giai": "ưetwet",\r\n  "ma_kh": "4",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "ử",\r\n  "ngay_van_chuyen": "2025-05-08T17:00:00Z",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 4,\r\n  "ma_so_thue": "32623623",\r\n  "thue": 7,\r\n  "tong_thanh_toan": 0.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "rew",\r\n  "dia_diem_giao": "ưetwet"\r\n}	15	2025-05-27 22:35:02.543215
303	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926"\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.50,\r\n  "tong_chiet_khau": 3313.50,\r\n  "tong_thue": 0.00\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.5,\r\n  "tong_chiet_khau": 3313.5,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-27 23:21:54.07394
331	dmkh	INSERT	{\r\n  "ma_kh": "5"\r\n}	\N	{\r\n  "ma_kh": "5",\r\n  "ten_kh": "Khách hàng test 2805",\r\n  "dia_chi": "Hà Nội",\r\n  "dien_thoai": "0593869535",\r\n  "mo_ta": "Để test",\r\n  "ma_so_thue": "0859386934"\r\n}	15	2025-05-28 07:28:07.575702
360	phieu_xuat	INSERT	{\r\n  "so_ct": "PX000018-280525155109"\r\n}	\N	{\r\n  "so_ct": "PX000018-280525155109",\r\n  "ngay_ct": "2025-05-28T08:51:09.374Z",\r\n  "dien_giai": null,\r\n  "ma_kh": "4",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 0,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "32623623",\r\n  "thue": 0,\r\n  "tong_thanh_toan": 7424.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": null,\r\n  "dia_diem_giao": null\r\n}	18	2025-05-28 15:51:29.134304
222	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 10:38:47.013973
223	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_dvt": "DVT06"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212817",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT06",\r\n  "so_luong_ton": 325,\r\n  "ngay_cap_nhat": "2025-05-27T03:25:58.593Z"\r\n}	15	2025-05-27 10:38:47.987565
224	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT02",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT02",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:02.465Z"\r\n}	15	2025-05-27 10:38:47.998425
225	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT08",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT08",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:05.969Z"\r\n}	15	2025-05-27 10:38:48.006921
226	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT10",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT10",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 2356,\r\n  "ngay_cap_nhat": "2025-05-27T03:26:09.977Z"\r\n}	15	2025-05-27 10:38:48.013447
253	dmvt	UPDATE	{\r\n  "ma_vt": "FDSDF"\r\n}	{\r\n  "ma_vt": "FDSDF",\r\n  "ma_loai_vt": "LT01",\r\n  "ten_vt": "SDFSDG",\r\n  "min_ton": 2,\r\n  "max_ton": 43,\r\n  "barcode": "1",\r\n  "url": "1",\r\n  "rong": 34.00,\r\n  "cao": 234.00,\r\n  "khoi_luong": 32.00,\r\n  "mau_sac": "Xanh",\r\n  "kieu_dang": "FSDSDF",\r\n  "trangthai": 1,\r\n  "mo_ta": "DSFSDF",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "FDSDF",\r\n  "ma_loai_vt": "LT01",\r\n  "ten_vt": "SDFSDG",\r\n  "min_ton": 22,\r\n  "max_ton": 43,\r\n  "barcode": "1",\r\n  "url": "1",\r\n  "rong": 34.0,\r\n  "cao": 234.0,\r\n  "khoi_luong": 32.0,\r\n  "mau_sac": "Xanh",\r\n  "kieu_dang": "FSDSDF",\r\n  "trangthai": 1,\r\n  "mo_ta": "DSFSDF",\r\n  "ma_dvt": "DVT03"\r\n}	15	2025-05-27 11:51:29.225032
275	dmkho	INSERT	{\r\n  "ma_kho": "KHO123test"\r\n}	\N	{\r\n  "ma_kho": "KHO123test",\r\n  "ten_kho": "test duyệt phiếu",\r\n  "mo_ta": null,\r\n  "dia_chi": "Hoài Đức"\r\n}	15	2025-05-27 22:56:56.4012
277	phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-270525225659"\r\n}	\N	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392Z",\r\n  "dien_giai": "test",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00Z",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.4,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 625215359307.4\r\n}	15	2025-05-27 22:57:47.821131
304	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:21:55.126374
332	dmncc	INSERT	{\r\n  "ma_ncc": "4"\r\n}	\N	{\r\n  "ma_ncc": "4",\r\n  "ten_ncc": "Nhà cung cấp test",\r\n  "dia_chi": "Hà Nội",\r\n  "ghi_chu": "Để test",\r\n  "dien_thoai": "0583968463",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "9583564964"\r\n}	15	2025-05-28 07:29:03.671718
227	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 10:56:39.939119
254	loai_vat_tu	UPDATE	{\r\n  "ma_loai_vt": "LT01"\r\n}	{\r\n  "ma_loai_vt": "LT01",\r\n  "ten_loai_vt": "Vật tư điện tử",\r\n  "mo_ta": "Các linh kiện điện tử như IC, transistor, diode.",\r\n  "trangthai": 0\r\n}	{\r\n  "ma_loai_vt": "LT01",\r\n  "ten_loai_vt": "Vật tư điện tử3",\r\n  "mo_ta": "Các linh kiện điện tử như IC, transistor, diode.",\r\n  "trangthai": 0\r\n}	15	2025-05-27 11:54:09.709389
276	ton_kho	INSERT	{\r\n  "ma_kho": "KHO123test",\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "PX000001-090525212816",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 24,\r\n  "ngay_cap_nhat": "2025-05-27T15:56:54.775Z"\r\n}	15	2025-05-27 22:56:56.495935
279	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 22:57:47.880149
305	phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-270525232753"\r\n}	\N	{\r\n  "so_ct": "PN000015-270525232753",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T16:27:53.344Z",\r\n  "dien_giai": "tset",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 0,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 53016.0,\r\n  "tong_chiet_khau": 2209.0,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-27 23:28:07.932017
333	dmncc	UPDATE	{\r\n  "ma_ncc": "4"\r\n}	{\r\n  "ma_ncc": "4",\r\n  "ten_ncc": "Nhà cung cấp test",\r\n  "dia_chi": "Hà Nội",\r\n  "ghi_chu": "Để test",\r\n  "dien_thoai": "0583968463",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "9583564964"\r\n}	{\r\n  "ma_ncc": "4",\r\n  "ten_ncc": "Nhà cung cấp test",\r\n  "dia_chi": "Hà Nội",\r\n  "ghi_chu": "Để test",\r\n  "dien_thoai": "0583968463",\r\n  "email": "mphucoca@gmail.com",\r\n  "ma_so_thue": "9583564964"\r\n}	15	2025-05-28 07:29:06.592828
228	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 253,\r\n  "ngay_cap_nhat": "2025-05-27T03:56:38.425Z"\r\n}	15	2025-05-27 10:56:41.023264
255	loai_vat_tu	UPDATE	{\r\n  "ma_loai_vt": "LSDF"\r\n}	{\r\n  "ma_loai_vt": "LSDF",\r\n  "ten_loai_vt": "test thôi",\r\n  "mo_ta": "đẹp",\r\n  "trangthai": 1\r\n}	{\r\n  "ma_loai_vt": "LSDF",\r\n  "ten_loai_vt": "test thôi",\r\n  "mo_ta": "đẹp 3",\r\n  "trangthai": 1\r\n}	15	2025-05-27 11:54:45.418949
278	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 22:57:47.867993
306	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000015-270525232753",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO01",\r\n  "ma_dvt": "DVT08"\r\n}	\N	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO01",\r\n  "so_ct": "PN000015-270525232753",\r\n  "ma_dvt": "DVT08",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:28:08.034135
334	loai_vat_tu	INSERT	{\r\n  "ma_loai_vt": "LOAI_VT_TEST"\r\n}	\N	{\r\n  "ma_loai_vt": "LOAI_VT_TEST",\r\n  "ten_loai_vt": "Loại vật tư test",\r\n  "mo_ta": "test",\r\n  "trangthai": 1\r\n}	15	2025-05-28 07:30:28.836501
229	dmkho	UPDATE	{\r\n  "ma_kho": "KHO02"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	{\r\n  "ma_kho": "KHO02",\r\n  "ten_kho": "Kho 02",\r\n  "mo_ta": "Kho thành phẩm hoàn chỉnh",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-27 11:02:50.600873
235	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 243,\r\n  "ngay_cap_nhat": "2025-05-27T04:03:09.729Z"\r\n}	15	2025-05-27 11:03:11.915928
256	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525165947"\r\n}	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:59:46.99",\r\n  "dien_giai": "ádfjhkjhsdf",\r\n  "trang_thai": 3,\r\n  "bien_so_xe": "235",\r\n  "ngay_van_chuyen": "2025-05-22T17:00:00",\r\n  "tt_thanhtoan": 6,\r\n  "chietkhau": 3,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 5,\r\n  "tong_thanh_toan": 113653.05,\r\n  "tong_chiet_khau": 2209.00,\r\n  "tong_thue": 5412.05\r\n}	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-25T09:59:46.99",\r\n  "dien_giai": "ádfjhkjhsdf 343",\r\n  "trang_thai": 3,\r\n  "bien_so_xe": "235",\r\n  "ngay_van_chuyen": "2025-05-22T17:00:00",\r\n  "tt_thanhtoan": 6,\r\n  "chietkhau": 3,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 5,\r\n  "tong_thanh_toan": 56826.525,\r\n  "tong_chiet_khau": 1104.5,\r\n  "tong_thue": 2706.025\r\n}	15	2025-05-27 11:58:34.813983
280	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659"\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.40,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 625215359307.40\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.4,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 625215359307.4\r\n}	15	2025-05-27 23:01:46.587656
282	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.00,\r\n  "ghi_chu": null\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 23:01:47.58097
284	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:03:40.304098
307	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926"\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.50,\r\n  "tong_chiet_khau": 3313.50,\r\n  "tong_thue": 0.00\r\n}	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_ncc": "NCC04",\r\n  "ngay_ct": "2025-05-25T09:49:26.83",\r\n  "dien_giai": "sgdsfg",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "236236",\r\n  "ngay_van_chuyen": "2025-05-16T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 7,\r\n  "ma_so_thue": "236236test",\r\n  "thue": 4,\r\n  "tong_thanh_toan": 51911.5,\r\n  "tong_chiet_khau": 3313.5,\r\n  "tong_thue": 0.0\r\n}	15	2025-05-28 00:05:34.762786
309	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937"\r\n}	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-10T14:39:37.456",\r\n  "dien_giai": "ets",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 2,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "sadgsadg",\r\n  "thue": 2,\r\n  "tong_thanh_toan": 608136.48,\r\n  "tong_chiet_khau": 24132.40,\r\n  "tong_thue": 28958.88\r\n}	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-10T14:39:37.456",\r\n  "dien_giai": "etsté",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": null,\r\n  "ngay_van_chuyen": null,\r\n  "tt_thanhtoan": 2,\r\n  "chietkhau": 5,\r\n  "ma_so_thue": "sadgsadg",\r\n  "thue": 2,\r\n  "tong_thanh_toan": 608136.48,\r\n  "tong_chiet_khau": 24132.4,\r\n  "tong_thue": 28958.88\r\n}	15	2025-05-28 00:06:00.469417
313	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO06",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 342,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO06",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 342,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 00:06:01.538059
319	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO05",\r\n  "ma_dvt": "DVT05"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT05",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "234"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT05",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "234"\r\n}	15	2025-05-28 00:06:01.585858
321	dmkh	UPDATE	{\r\n  "ma_kh": "KH07"\r\n}	{\r\n  "ma_kh": "KH07",\r\n  "ten_kh": "Công ty G",\r\n  "dia_chi": "Lào Cai",\r\n  "dien_thoai": "0907890123  ",\r\n  "mo_ta": "Khách hàng vùng cao",\r\n  "ma_so_thue": null\r\n}	{\r\n  "ma_kh": "KH07",\r\n  "ten_kh": "Công ty G",\r\n  "dia_chi": "Lào Cai",\r\n  "dien_thoai": "0907890124",\r\n  "mo_ta": "Khách hàng vùng cao",\r\n  "ma_so_thue": null\r\n}	15	2025-05-28 00:07:32.756817
335	dmvt	INSERT	{\r\n  "ma_vt": "VT_TEST01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_loai_vt": "LOAI_VT_TEST",\r\n  "ten_vt": "VT TEST 01",\r\n  "min_ton": 1,\r\n  "max_ton": 432,\r\n  "barcode": "1",\r\n  "url": "/Image/123.jpeg",\r\n  "rong": 23.0,\r\n  "cao": 43.0,\r\n  "khoi_luong": 24.0,\r\n  "mau_sac": "Đỏ",\r\n  "kieu_dang": "TEST",\r\n  "trangthai": 1,\r\n  "mo_ta": "TEST",\r\n  "ma_dvt": "DVT01"\r\n}	15	2025-05-28 07:32:19.293262
337	dmvt	INSERT	{\r\n  "ma_vt": "VT_TEST03"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_loai_vt": "LOAI_VT_TEST",\r\n  "ten_vt": "VT TEST 03",\r\n  "min_ton": 1,\r\n  "max_ton": 1,\r\n  "barcode": "1",\r\n  "url": null,\r\n  "rong": 1.0,\r\n  "cao": 1.0,\r\n  "khoi_luong": 1.0,\r\n  "mau_sac": "TEST",\r\n  "kieu_dang": "TEST",\r\n  "trangthai": 1,\r\n  "mo_ta": "TEST",\r\n  "ma_dvt": "DVT01"\r\n}	15	2025-05-28 07:34:17.711445
338	dmkho	INSERT	{\r\n  "ma_kho": "KHOTEST"\r\n}	\N	{\r\n  "ma_kho": "KHOTEST",\r\n  "ten_kho": "TEST KHO",\r\n  "mo_ta": "Để test",\r\n  "dia_chi": "Hà Nội"\r\n}	15	2025-05-28 07:36:20.999349
230	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "SDFSDF",\r\n  "ma_dvt": "DVT04"\r\n}	\N	{\r\n  "ma_vt": "SDFSDF",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT04",\r\n  "so_luong_ton": 234,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:45.297Z"\r\n}	15	2025-05-27 11:02:51.676371
257	ct_phieu_nhap	INSERT	{\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT07"\r\n}	\N	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO02",\r\n  "so_ct": "PN000011-250525165947",\r\n  "ma_dvt": "DVT07",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 11:58:36.20001
281	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 34,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-27 23:01:47.573763
283	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659"\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 0,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.40,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 625215359307.40\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.4,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 625215359307.4\r\n}	15	2025-05-27 23:03:39.269491
285	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO123test",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.00,\r\n  "ghi_chu": null\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO123test",\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 2645346,\r\n  "don_gia_nhap": 2363454.0,\r\n  "ghi_chu": null\r\n}	15	2025-05-27 23:03:40.311135
308	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_vt": "VT06",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT06",\r\n  "ma_ncc": "NCC04",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000011-250525164926",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 235,\r\n  "don_gia_nhap": 235.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 00:05:36.173372
310	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "sdg"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 324,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "sdg"\r\n}	15	2025-05-28 00:06:01.515099
316	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT05",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT01"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.00,\r\n  "ghi_chu": "235"\r\n}	{\r\n  "ma_vt": "VT05",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 234.0,\r\n  "ghi_chu": "235"\r\n}	15	2025-05-28 00:06:01.562277
336	dmvt	INSERT	{\r\n  "ma_vt": "VT_TEST02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_loai_vt": "LOAI_VT_TEST",\r\n  "ten_vt": "VT TEST 02",\r\n  "min_ton": 1,\r\n  "max_ton": 23,\r\n  "barcode": "1",\r\n  "url": null,\r\n  "rong": 2.0,\r\n  "cao": 3.0,\r\n  "khoi_luong": 2.0,\r\n  "mau_sac": "TEST",\r\n  "kieu_dang": "TEST",\r\n  "trangthai": 1,\r\n  "mo_ta": "TEST",\r\n  "ma_dvt": "DVT01"\r\n}	15	2025-05-28 07:33:41.698427
340	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_ton": 12534,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:26.645Z"\r\n}	15	2025-05-28 07:36:21.054501
343	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_ton": 12124,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:46.412Z"\r\n}	15	2025-05-28 07:36:21.081211
346	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_dvt": "DVT02"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT02",\r\n  "so_luong_ton": 1423,\r\n  "ngay_cap_nhat": "2025-05-28T00:36:18.652Z"\r\n}	15	2025-05-28 07:36:21.108071
231	ton_kho	INSERT	{\r\n  "ma_kho": "KHO02",\r\n  "ma_vt": "VT01",\r\n  "ma_dvt": "DVT03"\r\n}	\N	{\r\n  "ma_vt": "VT01",\r\n  "ma_kho": "KHO02",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_ton": 235,\r\n  "ngay_cap_nhat": "2025-05-27T04:02:48.817Z"\r\n}	15	2025-05-27 11:02:51.68889
258	phieu_xuat	UPDATE	{\r\n  "so_ct": "PX000001-250525135927"\r\n}	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ngay_ct": "2025-05-25T06:59:27.806",\r\n  "dien_giai": "236456",\r\n  "ma_kh": null,\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "2362362",\r\n  "ngay_van_chuyen": "2025-05-14T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "23563253",\r\n  "thue": 0,\r\n  "tong_thanh_toan": 410820.00,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 0.00,\r\n  "nguoi_giao": "236236",\r\n  "dia_diem_giao": "246236236"\r\n}	{\r\n  "so_ct": "PX000001-250525135927",\r\n  "ngay_ct": "2025-05-25T06:59:27.806",\r\n  "dien_giai": "236456",\r\n  "ma_kh": "2",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "2362362",\r\n  "ngay_van_chuyen": "2025-05-14T17:00:00",\r\n  "tt_thanhtoan": 5,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "23563253",\r\n  "thue": 0,\r\n  "tong_thanh_toan": 410820.0,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 0.0,\r\n  "nguoi_giao": "2362363",\r\n  "dia_diem_giao": "246236236"\r\n}	15	2025-05-27 12:04:31.58585
286	phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000015-270525225659"\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 2,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.40,\r\n  "tong_chiet_khau": 0.00,\r\n  "tong_thue": 625215359307.40\r\n}	{\r\n  "so_ct": "PN000015-270525225659",\r\n  "ma_ncc": "1",\r\n  "ngay_ct": "2025-05-27T15:56:59.392",\r\n  "dien_giai": "test",\r\n  "trang_thai": 1,\r\n  "bien_so_xe": "test",\r\n  "ngay_van_chuyen": "2025-05-09T17:00:00",\r\n  "tt_thanhtoan": 3,\r\n  "chietkhau": 0,\r\n  "ma_so_thue": "123125234235",\r\n  "thue": 3,\r\n  "tong_thanh_toan": 6877368952381.4,\r\n  "tong_chiet_khau": 0.0,\r\n  "tong_thue": 625215359307.4\r\n}	15	2025-05-27 23:06:35.374637
194	dmkho	DELETE	{\r\n  "ma_kho": "KHO01"\r\n}	{\r\n  "ma_kho": "KHO01",\r\n  "ten_kho": "Kho 01",\r\n  "mo_ta": "Test 1",\r\n  "dia_chi": "Thái Bình"\r\n}	\N	15	2025-05-27 09:52:29.62532
311	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT04",\r\n  "ma_kho": "KHO04",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 43,\r\n  "don_gia_nhap": 34.00,\r\n  "ghi_chu": "234"\r\n}	{\r\n  "ma_vt": "VT04",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO04",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 43,\r\n  "don_gia_nhap": 34.0,\r\n  "ghi_chu": "234"\r\n}	15	2025-05-28 00:06:01.521501
317	ct_phieu_nhap	UPDATE	{\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_vt": "VT07",\r\n  "ma_kho": "KHO05",\r\n  "ma_dvt": "DVT03"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.00,\r\n  "ghi_chu": "325"\r\n}	{\r\n  "ma_vt": "VT07",\r\n  "ma_ncc": "1",\r\n  "ma_kho": "KHO05",\r\n  "so_ct": "PN000001-100525213937",\r\n  "ma_dvt": "DVT03",\r\n  "so_luong_nhap": 234,\r\n  "don_gia_nhap": 324.0,\r\n  "ghi_chu": "325"\r\n}	15	2025-05-28 00:06:01.570291
339	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST01",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 124125,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:20.645Z"\r\n}	15	2025-05-28 07:36:21.044345
342	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST02",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 12524,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:41.588Z"\r\n}	15	2025-05-28 07:36:21.072704
345	ton_kho	INSERT	{\r\n  "ma_kho": "KHOTEST",\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_dvt": "DVT01"\r\n}	\N	{\r\n  "ma_vt": "VT_TEST03",\r\n  "ma_kho": "KHOTEST",\r\n  "ma_dvt": "DVT01",\r\n  "so_luong_ton": 12524,\r\n  "ngay_cap_nhat": "2025-05-28T00:35:57.668Z"\r\n}	15	2025-05-28 07:36:21.098563
\.


--
-- TOC entry 2099 (class 0 OID 0)
-- Dependencies: 205
-- Name: audit_log_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('audit_log_id_seq', 361, true);


--
-- TOC entry 2082 (class 0 OID 41019)
-- Dependencies: 195
-- Data for Name: ct_phieu_nhap; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY ct_phieu_nhap (ma_vt, ma_ncc, ma_kho, so_ct, so_luong_nhap, don_gia_nhap, ma_dvt, ghi_chu) FROM stdin;
VT06	NCC06	KHO05	PN006	180	15000.00	DVT04	Nhập phân bón
VT08	NCC08	KHO07	PN008	90	120000.00	DVT01	Nhập bóng đá
VT06	1	KHO123test	PN000015-270525225659	34	235.00	DVT03	235
VT07	1	KHO123test	PN000015-270525225659	2645346	2363454.00	DVT03	\N
VT07	1	KHO01	PN000015-270525232753	235	235.00	DVT08	235
VT06	NCC04	KHO04	PN000011-250525164926	235	235.00	DVT01	235
VT05	1	KHO04	PN000001-100525213937	324	234.00	DVT03	sdg
VT04	1	KHO04	PN000001-100525213937	43	34.00	DVT03	234
VT04	1	KHO04	PN000001-100525213937	432	234.00	DVT06	sd
VT05	1	KHO06	PN000001-100525213937	342	234.00	DVT01	235
VT06	1	KHO04	PN000001-100525213937	234	324.00	DVT01	23
VT04	1	KHO07	PN000001-100525213937	324	234.00	DVT06	dgsdg
VT05	1	KHO04	PN000001-100525213937	234	234.00	DVT01	235
VT07	1	KHO05	PN000001-100525213937	234	324.00	DVT03	325
VT06	1	KHO06	PN000001-100525213937	234	34.00	DVT03	sd
VT06	1	KHO05	PN000001-100525213937	234	234.00	DVT05	234
VT01	2	KHO04	PN000001-110525094725	234	234.00	DVT03	teset
VT05	1	KHO02	PN000011-250525165947	235	235.00	DVT07	235
VT_TEST01	4	KHOTEST	PN000015-280525074408	23554	235.00	DVT01	TEST
VT_TEST02	4	KHOTEST	PN000015-280525074408	35235	2523.00	DVT02	TEST2
VT04	NCC03	KHO05	PN000001-240525212028	123	2412.00	DVT06	Teset
VT05	NCC03	KHO05	PN000001-240525212028	2	32.00	DVT01	2532
VT07	NCC03	KHO05	PN000001-240525212028	3	423.00	DVT03	23523
VT06	NCC03	KHO05	PN000001-240525212028	23	235235.00	DVT01	235325
VT05	NCC03	KHO04	PN000001-240525212028	235	235.00	DVT01	235
VT05	1	KHO04	PN000001-250525163507	214	42532.00	DVT07	235
VT05	1	KHO03	PN000001-250525163507	243	4235.00	DVT03	5235
VT04	1	KHO04	PN000001-250525163507	235	235.00	DVT03	235
\.


--
-- TOC entry 2085 (class 0 OID 41043)
-- Dependencies: 198
-- Data for Name: ct_phieu_xuat; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY ct_phieu_xuat (ma_vt, ma_kho, so_ct, so_luong_xuat, don_gia_xuat, ma_kh, ma_dvt, ghi_chu) FROM stdin;
SDFDSF	KHO07	PX000001-090525232209	24	234.00	KH03	DVT05	235
SDFSDF	KHO05	PX000001-090525232209	24	234.00	KH03	DVT04	23sdgsd
VT05	KHO05	PX000001-090525232209	0	412.00	KH03	DVT03	4323
VT01	KHO01	PX000001-250525135927	12	34235.00	2	DVT02	523
VT07	KHO123test	PX000001-110525141456	235	235.00	1	DVT03	236
PX000001-090525212816	KHO123test	PX000001-110525141456	23	236.00	1	DVT04	235
VT_TEST02	KHOTEST	PX000015-270525223438	230	235236.00	4	DVT01	235
VT_TEST02	KHOTEST	PX000018-280525155109	232	32.00	4	DVT02	\N
PX000001-090525212817	KHO04	PX000000-090525230149	35	234.00	1	DVT06	235
VT04	KHO03	PX000001-100525213733	24	213.00	1	DVT06	2
VT04	KHO04	PX000001-100525213733	324	324.00	1	DVT06	34
VT04	KHO05	PX000001-100525213733	234	43.00	1	DVT04	23
VT04	KHO06	PX000001-100525213733	23	234.00	1	DVT06	sdg
VT08	KHO03	PX000001-100525213733	32	234.00	1	DVT04	423sdg
\.


--
-- TOC entry 2076 (class 0 OID 40977)
-- Dependencies: 189
-- Data for Name: dmdvt; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmdvt (ma_dvt, ten_dvt, mo_ta, trangthai) FROM stdin;
DVT01	Cái	Đơn vị đếm cho các vật phẩm nhỏ	1
DVT02	Hộp	Đơn vị đóng gói cho các vật phẩm	1
DVT04	Lít	Lít, đơn vị đo thể tích	1
DVT06	M3	Mét khối, đơn vị đo thể tích	1
DVT07	Cuộn	Đơn vị đóng gói cho vật liệu cuộn	0
DVT08	Thùng	Đơn vị đóng gói lớn	0
DVT03	Kg	Kilogram, đơn vị đdo khối lượng	1
SDFSDG	SDSDG	DSSDG	1
DVT05	M2	Mét vuông, đơn vị đo diện tích	1
dg	236	23643	1
\.


--
-- TOC entry 2083 (class 0 OID 41027)
-- Dependencies: 196
-- Data for Name: dmkh; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmkh (ma_kh, ten_kh, dia_chi, dien_thoai, mo_ta, ma_so_thue) FROM stdin;
KH04	Công ty D	Hải Phòng	0904567890  	Khách hàng vận tải	\N
KH08	Công ty H	Bắc Ninh	0908901234  	Khách hàng công nghiệp	\N
KH09	Công ty I	Bình Dương	0909012345  	Khách hàng sản xuất	\N
KH10	Công ty J	Nghệ An	0910123456  	Khách hàng thương mại	\N
KH03	Công ty C	Đà Nẵng Thái Bình	0903456789  	Khách hàng miền Trung	\N
KH02	Công ty B	TP.HCM	0902345678  	Khách hàng miền Nam	3t62323235
4	FSDFSDFSDF	5236236	235262352335	235235	32623623
KH01	Công ty A	Hà Nội	09012345673 	Khách hàng miền Bắc	\N
KH05	Công ty E	Cần Thơ	0905678901u 	Khách hàng nông nghiệp	\N
KH07	Công ty G	Lào Cai	0907890124  	Khách hàng vùng cao	\N
5	Khách hàng test 2805	Hà Nội	0593869535  	Để test	0859386934
\.


--
-- TOC entry 2079 (class 0 OID 40998)
-- Dependencies: 192
-- Data for Name: dmkho; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmkho (ma_kho, ten_kho, mo_ta, dia_chi) FROM stdin;
KHO01	Kho 01	Kho nguyên vật liệu sản xuất đúc	Thái Bình
KHO02	Kho 02	Kho thành phẩm hoàn chỉnh	Hà Nội
KHO123test	test duyệt phiếu	\N	Hoài Đức
KHOTEST	TEST KHO	Để test	Hà Nội
\.


--
-- TOC entry 2078 (class 0 OID 40990)
-- Dependencies: 191
-- Data for Name: dmncc; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmncc (ma_ncc, ten_ncc, dia_chi, ghi_chu, dien_thoai, email, ma_so_thue) FROM stdin;
NCC02	Công ty TNHH XYZ	TP.HCM	Nhà cung cấp vật liệu xây dựng	0987654321	xyz@company.com	\N
NCC04	Công ty TNHH GHI	Hải Phòng	Cung cấp vật tư y tế	0965432109	ghi@company.com	\N
NCC05	Công ty TNHH JKL	Cần Thơ	Chuyên phân phối phân bón	0954321098	jkl@company.com	\N
NCC06	Công ty TNHH MNO	Huế	Vật tư thể thao chính hãng	0943210987	mno@company.com	\N
NCC07	Công ty TNHH PQR	Quảng Ninh	Phụ tùng ô tô nhập khẩu	0932109876	pqr@company.com	\N
NCC09	Công ty TNHH VWX	Bình Dương	Thiết bị máy tính	0910987654	vwx@company.com	\N
NCC008	Công ty TNHH STU	Lâm Đồng	Cung cấp đồ dùng gia đình	0921098765	stu@company.com	\N
NCC001	Công ty TNHH ABC	Hà Nội	Nhà cung cấp linh kiện điện tử	0912345678	abc@company.com	\N
NCC10	Công ty TNHH YZ	Bắc Ninh	Dụng cụ học sinh	0909876543	yz@company.com	23523
NCC03	Công ty TNHH DEF	Đà Nẵng	Nhà cung cấp thiết bị văn phòng	0976543210	def@company.com	235235
2	sdf	sdf	sdf	sdf	\N	235235
3	FSDG2	Hưng Yên s	23326	095485495	mphucoca@gmail.com	23636235
1	Tên nhà cung cấp test	Thái Bình Hưng Yên s	Note thử test sửa	0384653035	mphucoca@gmail.com	123125234235
4	Nhà cung cấp test	Hà Nội	Để test	0583968463	mphucoca@gmail.com	9583564964
\.


--
-- TOC entry 2087 (class 0 OID 41076)
-- Dependencies: 200
-- Data for Name: dmqddvt; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmqddvt (ma_vt, ma_dvt, ty_le_quy_doi) FROM stdin;
VT01	DVT01	1.00
VT01	DVT02	10.00
VT02	DVT01	1.00
VT02	DVT03	0.50
VT05	DVT01	1.00
VT05	DVT07	20.00
VT07	DVT01	1.00
VT07	DVT08	5.00
VT08	DVT01	1.00
VT08	DVT04	0.50
VT09	DVT01	1.00
VT09	DVT05	2.00
VT10	DVT01	1.00
VT10	DVT02	12.00
VT06	DVT01	1.00
VT06	DVT03	0.20
VT06	DVT04	34.00
VT06	DVT05	24.00
SDFSDF	DVT02	43.00
SDFSDF	DVT03	43.00
SDFSDF	DVT04	34.00
VT04	DVT04	1.00
VT04	DVT06	100.00
SDFDSF	DVT04	24.00
SDFDSF	DVT05	32.00
VT03	DVT01	1.00
VT03	DVT05	100.00
GSGSDG	DVT04	34.00
PX000001-090525212817	DVT06	34.00
PX000001-090525212816	DVT03	234.00
VT25235	DVT04	12.00
FGFHFH	DVT03	235.00
FGFHFH	DVT04	235.00
VT02359	DVT05	43.00
FDSDF	DVT04	3.00
VT_TEST01	DVT02	1.00
VT_TEST01	DVT03	2.00
VT_TEST02	DVT02	1.00
VT_TEST02	DVT03	1.00
VT_TEST03	DVT02	1.00
VT_TEST03	DVT03	1.00
\.


--
-- TOC entry 2077 (class 0 OID 40985)
-- Dependencies: 190
-- Data for Name: dmvt; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY dmvt (ma_vt, ma_loai_vt, ten_vt, min_ton, max_ton, barcode, url, rong, cao, khoi_luong, mau_sac, kieu_dang, trangthai, mo_ta, ma_dvt) FROM stdin;
GSGSDG	LT04	DGSG	234	34	1	1	34.00	34.00	54.00	GSDG	35	1	325	DVT03
SDFSDF	LT03	fdfsdf	2	324	1	1	23.00	23.00	54.00	24	325	1	324	DVT03
VT01	LT03	IC 555	10	100	1234567890	http://example.com/ic555	2.50	1.50	0.05	FSDFSD	235	1	agasdgdf	DVT03
VT02	LT02	Ốc vít M5	50	500	1234567891	http://example.com/ocvitm5	1.00	0.50	0.02	Đỏ d	da	1	test sửa	DVT03
VT06	LT06	Phân bón NPK	100	1000	1234567895	http://example.com/phanbonNPK	20.00	20.00	25.00	255.00	255.00	0	\N	DVT03
VT08	LT08	Bóng đá	50	500	1234567897	http://example.com/bongda	22.00	22.00	0.50	255.00	255.00	0	\N	DVT03
VT10	LT10	Chuột máy tính	100	1000	1234567899	http://example.com/chuot	10.00	5.00	0.20	255.00	255.00	1	\N	DVT03
VT09	LT09	Lốp xe ô tô	20	200	1234567898	http://example.com/lopxe	60.00	60.00	10.00	255.00	255.00	0	\N	DVT03
VT07	LT07	Nồi cơm điện	10	100	1234567896	http://example.com/noicome	30.00	30.00	2.00	255.00	255.00	0	\N	DVT03
VT05	LT05	Bông y tế	50	500	1234567894	http://example.com/bongyte	10.00	10.00	0.10	255.00	255.00	1	\N	DVT03
GSFSDFGSD	LT02	235	42	3	1	/Image/Image20250511160848.png	24.00	43.00	23.00	42	24	1	43224	DVT03
PX000001-090525212817	LT04	dg	34	234	1	/Image/123.jpeg	235.00	235.00	235.00	235	235	0	235	DVT05
PX000001-090525212816	LT03	ưefsdf	34	23	1	/Image/thiet-ke-khuon-nhua-4.jpeg	23.00	325.00	234.00	23	235	0	234	DVT04
VT04	LT04	Xi măng PC40	200	2000	1234567893	http://example.com/ximangPC40	30.00	30.00	50.00	200.00	200.00	1	\N	DVT03
VT25235	LT02	Testn	2	532	1	/Image/123.jpeg	24.00	423.00	423.00	Đỏ	Dnga	1	Test	DVT06
SDFDSF	LT01	ádga	23	34	1	1	235.00	234.00	324.00	34	2352	1	234	DVT03
VT02359	LT01	test 124	12	14	1	/Image/123.jpeg	34.00	43.00	43.00	43	GDSGF	1	236	DVT04
FDSDF	LT01	SDFSDG	22	43	1	1	34.00	234.00	32.00	Xanh	FSDSDF	1	DSFSDF	DVT03
VT_TEST01	LOAI_VT_TEST	VT TEST 01	1	432	1	/Image/123.jpeg	23.00	43.00	24.00	Đỏ	TEST	1	TEST	DVT01
VT_TEST02	LOAI_VT_TEST	VT TEST 02	1	23	1	\N	2.00	3.00	2.00	TEST	TEST	1	TEST	DVT01
VT_TEST03	LOAI_VT_TEST	VT TEST 03	1	1	1	\N	1.00	1.00	1.00	TEST	TEST	1	TEST	DVT01
\.


--
-- TOC entry 2075 (class 0 OID 40969)
-- Dependencies: 188
-- Data for Name: loai_vat_tu; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY loai_vat_tu (ma_loai_vt, ten_loai_vt, mo_ta, trangthai) FROM stdin;
LT04	Vật tư xây dựng	Vật liệu xây dựng như xi măng, cát, đá.	1
LT06	Vật tư nông nghiệp	Sản phẩm phục vụ nông nghiệp như phân bón, hạt giống.	1
LT05	Vật tư y tế	Dụng cụ y tế như bông băng, thuốc, kim tiêm.	1
LT07	Vật tư gia dụng	Đồ dùng gia đình như nồi, chảo, bát đĩa.	1
LT08	Vật tư thể thao	Dụng cụ thể thao như bóng đá, vợt cầu lông.	0
LT10	Vật tư máy tính	Phụ kiện máy tính như chuột, bàn phím, ổ cứng.	0
LT03	Vật tư văn phòng	Dụng cụ văn phòng như giấy, bút, kẹp giấy.	0
LT02	Vật tư cơ khí test35	Các sản phẩm cơ khí như ốc vít, bulong, đai ốc.	0
LT053	23542	23655	1
LT01	Vật tư điện tử3	Các linh kiện điện tử như IC, transistor, diode.	0
LSDF	test thôi	đẹp 3	1
LOAI_VT_TEST	Loại vật tư test	test	1
\.


--
-- TOC entry 2081 (class 0 OID 41011)
-- Dependencies: 194
-- Data for Name: phieu_nhap; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY phieu_nhap (so_ct, ma_ncc, ngay_ct, dien_giai, trang_thai, bien_so_xe, ngay_van_chuyen, tt_thanhtoan, chietkhau, ma_so_thue, thue, tong_thanh_toan, tong_chiet_khau, tong_thue) FROM stdin;
PN000015-280525074408	4	2025-05-28 00:44:08.829	TEST 1	0	12-MD543	2025-05-15 17:00:00	1	1	9583564964	1	94433095.00	0.00	0.00
PN000001-240525212028	NCC03	2025-05-24 14:20:28.362	Test nhập dữ liệu sdfds	1	14n5-235u9 sdg	2025-05-21 17:00:00	4	9	235235	12	5567675.27	461091.12	265127.39
PN000001-110525094725	2	2025-05-11 02:47:25.285	gádg	0	23523	2025-05-14 17:00:00	\N	\N	2352353sdgsdet	\N	\N	\N	\N
PN000001-250525163507	1	2025-05-25 09:35:07.798	test khởi tạo và lưu log	1	kfjasdfkjkl	2025-05-02 17:00:00	3	5	123125234235	4	9778730.88	407447.12	0.00
PN000015-270525225659	1	2025-05-27 15:56:59.392	test	2	test	2025-05-09 17:00:00	3	0	123125234235	3	6877368952381.40	0.00	625215359307.40
PN000015-270525232753	1	2025-05-27 16:27:53.344	tset	0	test	\N	0	5	123125234235	4	53016.00	2209.00	0.00
PN000011-250525164926	NCC04	2025-05-25 09:49:26.83	sgdsfg	0	236236	2025-05-16 17:00:00	5	7	236236test	4	51911.50	3313.50	0.00
PN000001-100525213937	1	2025-05-10 14:39:37.456	etsté	2	\N	\N	2	5	sadgsadg	2	608136.48	24132.40	28958.88
PN000011-250525165947	1	2025-05-25 09:59:46.99	ádfjhkjhsdf 343	3	235	2025-05-22 17:00:00	6	3	123125234235	5	56826.53	1104.50	2706.03
\.


--
-- TOC entry 2084 (class 0 OID 41035)
-- Dependencies: 197
-- Data for Name: phieu_xuat; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY phieu_xuat (so_ct, ngay_ct, dien_giai, ma_kh, trang_thai, bien_so_xe, ngay_van_chuyen, tt_thanhtoan, chietkhau, ma_so_thue, thue, tong_thanh_toan, tong_chiet_khau, tong_thue, nguoi_giao, dia_diem_giao) FROM stdin;
PX000001-100525213733	2025-05-10 14:37:33.92	Test dữ liệu  bảng kê	1	1	\N	2025-05-08 17:00:00	7	7	\N	6	217153.44	12600.78	19741.22	\N	sdg
PX000001-090525232209	2025-05-09 16:22:09.412	test test test	KH03	0	ádgasdg	2025-05-22 17:00:00	6	5	235235	5	11321.86	449.28	539.14	ádgasdg	test
PX000001-110525140242	2025-05-11 07:02:42.865	dsgsdg	1	0	\N	\N	\N	4	\N	5	0.00	0.00	0.00	\N	\N
PX000001-250525135927	2025-05-25 06:59:27.806	236456	2	1	2362362	2025-05-14 17:00:00	5	0	235235	0	410820.00	0.00	0.00	2362363	246236236
PX000001-110525141456	2025-05-11 07:14:56.889	ádgsdg	1	2	3532sdf	2025-05-15 17:00:00	4	7	25235sđf	4	57013.82	3639.18	0.00	dsgadsg	sdgsdg
PX000000-090525230149	2025-05-09 16:01:49.537	\N	1	3	13252351sdf	2025-05-14 17:00:00	5	5	\N	4	\N	\N	\N	\N	\N
PX000015-270525223438	2025-05-27 15:34:38.911	ưetwet	4	2	ử	2025-05-08 17:00:00	5	4	32623623	7	60353324.34	1623128.40	7872172.74	rew	ưetwet
PX000018-280525155109	2025-05-28 08:51:09.374	\N	4	2	\N	\N	0	0	32623623	0	7424.00	0.00	0.00	\N	\N
\.


--
-- TOC entry 2080 (class 0 OID 41006)
-- Dependencies: 193
-- Data for Name: tonkho; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY tonkho (ma_vt, ma_kho, so_luong_ton, ma_dvt, ngay_cap_nhat) FROM stdin;
SDFSDF	KHO02	243	DVT03	2025-05-27 04:03:09.729
SDFSDF	KHO02	234	DVT04	2025-05-27 04:02:45.297
VT01	KHO02	5446	DVT03	2025-05-27 04:11:45.168
VT01	KHO02	235	DVT02	2025-05-27 04:08:36.202
VT02	KHO02	3532	DVT01	2025-05-27 04:11:39.416
VT06	KHO123test	136	DVT03	2025-05-27 23:09:58.53
VT05	KHO04	324	DVT03	2025-05-27 23:27:39.086
VT04	KHO04	43	DVT03	2025-05-27 23:27:39.086
VT04	KHO04	432	DVT06	2025-05-27 23:27:39.086
VT05	KHO06	342	DVT01	2025-05-27 23:27:39.086
VT06	KHO04	704	DVT01	2025-05-27 23:27:39.086
VT04	KHO07	324	DVT06	2025-05-27 23:27:39.086
VT05	KHO04	234	DVT01	2025-05-27 23:27:39.086
VT07	KHO05	234	DVT03	2025-05-27 23:27:39.086
VT06	KHO06	234	DVT03	2025-05-27 23:27:39.086
VT06	KHO05	234	DVT05	2025-05-27 23:27:39.086
VT07	KHO123test	10580914	DVT03	2025-05-28 00:20:48.209
PX000001-090525212816	KHO123test	-22	DVT04	2025-05-28 00:20:48.209
VT_TEST01	KHOTEST	12534	DVT02	2025-05-28 00:35:26.645
VT_TEST01	KHOTEST	125142	DVT03	2025-05-28 00:35:36.277
VT_TEST02	KHOTEST	12524	DVT01	2025-05-28 00:35:41.588
VT_TEST03	KHOTEST	1254	DVT03	2025-05-28 00:35:52.117
VT_TEST03	KHOTEST	12524	DVT01	2025-05-28 00:35:57.668
VT_TEST03	KHOTEST	1423	DVT02	2025-05-28 00:36:18.652
VT_TEST01	KHOTEST	147679	DVT01	2025-05-28 07:45:37.764
VT_TEST02	KHOTEST	47127	DVT02	2025-05-28 15:51:47.679
\.


--
-- TOC entry 2086 (class 0 OID 41051)
-- Dependencies: 199
-- Data for Name: userinfo; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY userinfo (id, username, fullname, password, role, email, trangthai) FROM stdin;
4	user3	Lê Văn C	user123	1	mphucoca@gmail.com	1
5	user4	Phạm Thị D	user123	1	\N	1
7	user6	Ngô Thị F	user123	1	\N	0
8	user7	Vũ Văn G	user123	1	\N	1
9	user9	Lý Văn I	user123	1	\N	1
3	user2	Trần Thị B	user12367	1	test@gmail.com	1
6	user5	Đỗ Văn E s	user123	1	\N	1
11	PHUC2	Đào Phúc	123456	1	mphucoca@gmail.com	1
12	testsgdgdf	sgdsgsd	têtsfd	0	sgsdg@gmail.com	1
15	PHUC1	Đào Phúc	123456	0	mphucoca@gmail.com	1
16	test	236236	23523623	1	23623@gmail.com	1
17	TEST1123	235	8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92	0	\N	1
18	PHUCTEST1	PHUCTEST1	8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92	0	mphucoca@gmail.com	1
19	PHUCTEST2	PHUCTEST2	8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92	1	mphucoca@gmail.com	1
\.


--
-- TOC entry 1964 (class 2606 OID 73774)
-- Name: audit_log_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY audit_log
    ADD CONSTRAINT audit_log_pkey PRIMARY KEY (id);


--
-- TOC entry 1952 (class 2606 OID 41026)
-- Name: ct_phieu_nhap_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY ct_phieu_nhap
    ADD CONSTRAINT ct_phieu_nhap_pkey PRIMARY KEY (ma_vt, ma_ncc, ma_kho, so_ct, ma_dvt);


--
-- TOC entry 1958 (class 2606 OID 41050)
-- Name: ct_phieu_xuat_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY ct_phieu_xuat
    ADD CONSTRAINT ct_phieu_xuat_pkey PRIMARY KEY (ma_vt, ma_kho, so_ct, ma_kh, ma_dvt);


--
-- TOC entry 1940 (class 2606 OID 40984)
-- Name: dmdvt_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmdvt
    ADD CONSTRAINT dmdvt_pkey PRIMARY KEY (ma_dvt);


--
-- TOC entry 1954 (class 2606 OID 41034)
-- Name: dmkh_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmkh
    ADD CONSTRAINT dmkh_pkey PRIMARY KEY (ma_kh);


--
-- TOC entry 1946 (class 2606 OID 41005)
-- Name: dmkho_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmkho
    ADD CONSTRAINT dmkho_pkey PRIMARY KEY (ma_kho);


--
-- TOC entry 1944 (class 2606 OID 40997)
-- Name: dmncc_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmncc
    ADD CONSTRAINT dmncc_pkey PRIMARY KEY (ma_ncc);


--
-- TOC entry 1962 (class 2606 OID 41080)
-- Name: dmqddvt_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmqddvt
    ADD CONSTRAINT dmqddvt_pkey PRIMARY KEY (ma_vt, ma_dvt);


--
-- TOC entry 1938 (class 2606 OID 40976)
-- Name: loai_vat_tu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY loai_vat_tu
    ADD CONSTRAINT loai_vat_tu_pkey PRIMARY KEY (ma_loai_vt);


--
-- TOC entry 1950 (class 2606 OID 41018)
-- Name: phieu_nhap_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY phieu_nhap
    ADD CONSTRAINT phieu_nhap_pkey PRIMARY KEY (so_ct, ma_ncc);


--
-- TOC entry 1956 (class 2606 OID 41042)
-- Name: phieu_xuat_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY phieu_xuat
    ADD CONSTRAINT phieu_xuat_pkey PRIMARY KEY (so_ct, ma_kh);


--
-- TOC entry 1942 (class 2606 OID 41082)
-- Name: pk_ma_vt; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY dmvt
    ADD CONSTRAINT pk_ma_vt PRIMARY KEY (ma_vt);


--
-- TOC entry 1948 (class 2606 OID 41010)
-- Name: tonkho_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY tonkho
    ADD CONSTRAINT tonkho_pkey PRIMARY KEY (ma_vt, ma_kho, ma_dvt);


--
-- TOC entry 1960 (class 2606 OID 41055)
-- Name: userinfo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY userinfo
    ADD CONSTRAINT userinfo_pkey PRIMARY KEY (id);


--
-- TOC entry 1966 (class 2620 OID 41058)
-- Name: trg_delete_ct_phieu_nhap; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_delete_ct_phieu_nhap AFTER DELETE ON phieu_nhap FOR EACH ROW EXECUTE PROCEDURE delete_ct_phieu_nhap();


--
-- TOC entry 1967 (class 2620 OID 49182)
-- Name: trg_delete_ct_phieu_xuat; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_delete_ct_phieu_xuat AFTER DELETE ON phieu_xuat FOR EACH ROW EXECUTE PROCEDURE delete_ct_phieu_xuat();


--
-- TOC entry 1965 (class 2620 OID 73783)
-- Name: trg_delete_tonkho_after_dmkho; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_delete_tonkho_after_dmkho AFTER DELETE ON dmkho FOR EACH ROW EXECUTE PROCEDURE delete_tonkho_when_dmkho_deleted();


--
-- TOC entry 2096 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2025-05-30 23:44:04

--
-- PostgreSQL database dump complete
--

