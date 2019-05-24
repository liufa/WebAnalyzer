--select  *,
--        GEOGRAPHY::STGeomFromText('POINT('+ 
--             SUBSTRING([Address], LEN(SUBSTRING([Address], 0, LEN([Address]) - CHARINDEX (',', [Address]))) + 1, 
--    LEN([Address]) - LEN(SUBSTRING([Address], 0, LEN([Address]) - CHARINDEX (',', [Address]))) - LEN(SUBSTRING(
--    col, CHARINDEX ('.', col), LEN(col))));convert(nvarchar(20), Longitude)+' '+
--            convert( nvarchar(20), Latitude)+')', 4326)
--        .STBuffer(Radius * 1000).STIntersects(@p) as [Intersects]

DECLARE  @i int = 1
declare @temp table(
[Id] [int] IDENTITY(1,1)  NOT NULL,
	[SaleId] [int] NULL,
	--[RentId] [int] NULL,
	[Point] [geography] NULL,
	[RentsIn1kRadiusCount] [int] NULL,
	[SalesIn1kRadiusCount] [int] NULL,
	[RentsIn1kRadiusAvgSqM] [decimal](12, 6) NULL,
	[SalesIn1kRadiusAvgSqM] [decimal](12, 6) NULL--,
	--[RentsIn500RadiusCount] [int] NULL,
	--[SalesIn500RadiusCount] [int] NULL,
	--[RentsIn500RadiusAvgSqM] [decimal](12, 6) NULL,
	--[SalesIn500RadiusAvgSqM] [decimal](12, 6) NULL,
	--[RentsIn200RadiusCount] [int] NULL,
	--[SalesIn200RadiusCount] [int] NULL,
	--[RentsIn200RadiusAvgSqM] [decimal](12, 6) NULL,
	--[SalesIn200RadiusAvgSqM] [decimal](12, 6) NULL,
	--[RentsIn100RadiusCount] [int] NULL,
	--[SalesIn100RadiusCount] [int] NULL,
	--[RentsIn100RadiusAvgSqM] [decimal](12, 6) NULL,
	--[SalesIn100RadiusAvgSqM] [decimal](12, 6) NULL
)

declare @lat nvarchar(20)
declare @lon nvarchar(20)
declare @rooms int
while @i <= 100--(select max(id) from Webanalyzer.dbo.sale)
begin

 select top 1 
 @lat = SUBSTRING(Address,0,CHARINDEX(',',Address,0)), 
 @lon = SUBSTRING(Address,CHARINDEX(',',Address)+1,LEN(Address)),
 @rooms = RoomCount from Webanalyzer.dbo.sale
		where id  = @i; 



declare @p GEOGRAPHY =  GEOGRAPHY::STGeomFromText('POINT('+ @lat +' '+   @lon        +')', 4326)


-------1k
		
	insert into	@temp([SaleId],[Point],[SalesIn1kRadiusCount],[SalesIn1kRadiusAvgSqM])
select r.id,  @p, cnt,   avg_sq_m  from
	(select 	(sum(price)/sum(LivingArea)) avg_sq_m,  count(1) cnt, @i id from 
			(select  *, GEOGRAPHY::STGeomFromText('POINT('+ 
            convert(nvarchar(20), SUBSTRING(Address,0,CHARINDEX(',',Address,0)))+' '+
            convert( nvarchar(20), SUBSTRING(Address,CHARINDEX(',',Address)+1,LEN(Address)))+')', 4326)
				.STBuffer(1000).STIntersects(@p) as [Intersects]
				from Webanalyzer.dbo.sale
				  where --(Title = 'gandia' or Title = 'daimus')   and
 address is not null and LivingArea is not null and RoomCount = @rooms
			) s
		where [Intersects] = 1) prox
		inner join Webanalyzer.dbo.sale r on  prox.id = r.id

update t set RentsIn1kRadiusCount = cnt, [RentsIn1kRadiusAvgSqM] =
		   avg_sq_m  from
	(select 	(sum(price)/sum(LivingArea)) avg_sq_m,  count(1) cnt, @i id from 
			(select  *, GEOGRAPHY::STGeomFromText('POINT('+ 
            convert(nvarchar(20), SUBSTRING(Address,0,CHARINDEX(',',Address,0)))+' '+
            convert( nvarchar(20), SUBSTRING(Address,CHARINDEX(',',Address)+1,LEN(Address)))+')', 4326)
				.STBuffer(1000).STIntersects(@p) as [Intersects]
				from Webanalyzer.dbo.rent
				  where --(Title = 'gandia' or Title = 'daimus')   and
 address is not null and LivingArea is not null and RoomCount is not null and RoomCount = @rooms
			) s
		where [Intersects] = 1) prox, @temp t
		where prox.id = t.Id

------500m

update t set RentsIn1kRadiusCount = cnt, [RentsIn1kRadiusAvgSqM] =
		   avg_sq_m  from
	(select 	(sum(price)/sum(LivingArea)) avg_sq_m,  count(1) cnt, @i id from 
			(select  *, GEOGRAPHY::STGeomFromText('POINT('+ 
            convert(nvarchar(20), SUBSTRING(Address,0,CHARINDEX(',',Address,0)))+' '+
            convert( nvarchar(20), SUBSTRING(Address,CHARINDEX(',',Address)+1,LEN(Address)))+')', 4326)
				.STBuffer(1000).STIntersects(@p) as [Intersects]
				from Webanalyzer.dbo.rent
				  where --(Title = 'gandia' or Title = 'daimus')   and
 address is not null and LivingArea is not null and RoomCount is not null and RoomCount = @rooms
			) s
		where [Intersects] = 1) prox, @temp t
		where prox.id = t.Id
		--inner join Webanalyzer.dbo.sale r on  prox.id = r.id
	--	group by s.Id
		set @i = @i+1
		--select 
		--SUBSTRING(Address,0,CHARINDEX(',',Address,0)) lat, SUBSTRING(Address,CHARINDEX(',',Address)+1,LEN(Address)) lon
	end	

	select * from @temp
	where [SalesIn1kRadiusCount] > 1
	-- order by ratio