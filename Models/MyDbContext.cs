using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public DbSet<Event> Events { set; get; }
    public DbSet<Payment> Payments { set; get; }
    public DbSet<Seat> Seats { set; get; }
    public DbSet<SeatGroup> SeatGroups { set; get; }
    public DbSet<SeatingChart> SeatingCharts { set; get; }
    public DbSet<Ticket> Tickets { set; get; }
    public DbSet<TicketGroup> TicketGroups { set; get; }
    public DbSet<User> Users { set; get; }
    public DbSet<Refund> Refunds{ set; get; }
    public DbSet<TicketDetail> TicketDetails{ set; get; }
    public DbSet<ShowTime> ShowTimes{ set; get; }
    public DbSet<ShowTimeSeat>ShowTimeSeats{ set; get; }
    public DbSet<ShowTimeTicketGroup> ShowTimeTicketGroups{ set; get; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
        .Property(tk => tk.Status)
        .HasConversion<string>();

        modelBuilder.Entity<Event>()
        .Property(ev => ev.EventStatus)
        .HasConversion<string>();

        modelBuilder.Entity<Event>()
        .Property(e => e.EventType)
        .HasConversion<string>();

        modelBuilder.Entity<Payment>()
        .Property(p => p.Status)
        .HasConversion<string>();

        modelBuilder.Entity<Refund>()
        .Property(r => r.Status)
        .HasConversion<string>();

        modelBuilder.Entity<Ticket>()
        .HasOne(tk => tk.User)
        .WithMany(us => us.Tickets)
        .HasForeignKey(tk => tk.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Ticket>()
        .HasOne(tk => tk.Event)
        .WithMany(e => e.Tickets)
        .HasForeignKey(tk => tk.EventId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Refund>()
        .HasOne(r => r.Ticket)
        .WithOne(t => t.Refund)
        .HasForeignKey<Refund>(r => r.TicketId)
        .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<TicketDetail>()
        .HasOne(td => td.Ticket)
        .WithMany(tk => tk.TicketDetails)
        .HasForeignKey(td => td.TicketId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketDetail>()
        .HasOne(td => td.ShowTimeSeat)
        .WithOne(s => s.TicketDetail)
        .HasForeignKey<TicketDetail>(td => new { td.ShowTimeId,td.SeatId })
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
        .HasOne(pm => pm.Ticket)
        .WithOne(tk => tk.Payment)
        .HasForeignKey<Payment>(pm => pm.TicketId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
        .HasOne(e => e.User)
        .WithMany(us => us.Events)
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SeatingChart>()
        .HasOne(stc => stc.Event)
        .WithOne(e => e.SeatingChart)
        .HasForeignKey<SeatingChart>(stc => stc.SeatingChartId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatGroup>()
        .HasOne(sg => sg.SeatingChart)
        .WithMany(stc => stc.SeatGroups)
        .HasForeignKey(sg => sg.SeatingChartId)
         .OnDelete(DeleteBehavior.NoAction); ;

        modelBuilder.Entity<Seat>()
        .HasOne(s => s.SeatGroup)
        .WithMany(sg => sg.Seats)
        .HasForeignKey(s => s.SeatGroupId)
         .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<TicketGroup>()
        .HasMany(tkg => tkg.SeatGroups)
        .WithOne(sg => sg.TicketGroup)
        .HasForeignKey(tkg => tkg.TicketGroupId)
        .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<ShowTime>()
        .HasOne(st => st.Event)
        .WithMany(ev => ev.ShowTimes)
        .HasForeignKey(st => st.EventId);

        modelBuilder.Entity<ShowTimeSeat>()
        .HasKey(sts => new { sts.ShowTimeId, sts.SeatId });

        modelBuilder.Entity<ShowTimeSeat>()
        .HasOne(sts => sts.ShowTime)
        .WithMany(st => st.ShowTimeSeats)
        .HasForeignKey(sts => sts.ShowTimeId);

        modelBuilder.Entity<ShowTimeSeat>()
        .HasOne(sts => sts.Seat)
        .WithMany(s => s.ShowTimeSeats)
        .HasForeignKey(sts => sts.SeatId);

        modelBuilder.Entity<ShowTimeTicketGroup>()
        .HasKey(stk => new { stk.ShowTimeId, stk.TicketGruopId });

        modelBuilder.Entity<ShowTimeTicketGroup>()
        .HasOne(stk => stk.ShowTime)
        .WithMany(st => st.ShowTimeTicketGroups)
        .HasForeignKey(stk => stk.ShowTimeId);

        modelBuilder.Entity<ShowTimeTicketGroup>()
        .HasOne(stk => stk.TicketGroup)
        .WithMany(tkg => tkg.ShowTimeTicketGroups)
        .HasForeignKey(stk => stk.TicketGruopId);

    }   
}