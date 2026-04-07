import styles from "./page.module.css";

export default function Home() {
  return (
    <div className={styles.page}>
      <main className={styles.main}>
        <div className={styles.hero}>
          <p className={styles.kicker}>Platform Bootstrap</p>
          <h1>Procont Cloud Core</h1>
          <p>
            Base tecnica para un SaaS contable multi-tenant con foco en arquitectura modular,
            seguridad por defecto y entrega continua.
          </p>
        </div>
        <div className={styles.grid}>
          <article>
            <h2>API</h2>
            <p>ASP.NET Core .NET 8 en capas: Api, Application, Domain, Infrastructure.</p>
          </article>
          <article>
            <h2>Web</h2>
            <p>Next.js + TypeScript con estandares de lint y formato para el equipo.</p>
          </article>
          <article>
            <h2>Shared</h2>
            <p>Contratos DTO compartidos para tenants, companias y usuarios.</p>
          </article>
          <article>
            <h2>Infra</h2>
            <p>Docker Compose con PostgreSQL y Redis para entorno local reproducible.</p>
          </article>
        </div>
      </main>
    </div>
  );
}
