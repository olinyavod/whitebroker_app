import {
  IonContent,
  IonHeader,
  IonPage,
  IonTitle,
  IonToolbar,
  IonCard,
  IonCardHeader,
  IonCardTitle,
  IonCardSubtitle,
  IonCardContent,
  IonButton,
  IonIcon,
  IonGrid,
  IonRow,
  IonCol,
  IonChip,
  IonLabel,
} from '@ionic/react';
import { useHistory } from 'react-router-dom';
import {
  logoReact,
  logoAndroid,
  logoApple,
  desktopOutline,
  globeOutline,
  rocketOutline,
  openOutline,
  phonePortraitOutline,
} from 'ionicons/icons';
import './Home.css';

interface AppInfo {
  name: string;
  description: string;
  framework: string;
  icon: string;
  platforms: string[];
  color: string;
  devPort?: number;
  status: 'running' | 'stopped' | 'building';
  urlPath: string;
}

const apps: AppInfo[] = [
  {
    name: 'Flutter App',
    description: 'White Broker приложение на Flutter с WebView',
    framework: 'Flutter',
    icon: 'https://storage.googleapis.com/cms-storage-bucket/4fd0db61df0567c0f352.png',
    platforms: ['iOS', 'Android', 'Web', 'Desktop'],
    color: '#02569B',
    devPort: 8080,
    status: 'stopped',
    urlPath: 'flutter',
  },
  {
    name: '.NET MAUI App',
    description: 'White Broker приложение на .NET MAUI',
    framework: '.NET MAUI',
    icon: 'https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg',
    platforms: ['iOS', 'Android', 'Windows', 'macOS'],
    color: '#512BD4',
    status: 'stopped',
    urlPath: 'maui',
  },
  {
    name: 'Ionic React App',
    description: 'White Broker приложение на Ionic React (текущее)',
    framework: 'Ionic React',
    icon: 'https://ionicframework.com/docs/icons/logo-react-icon.png',
    platforms: ['iOS', 'Android', 'Web', 'PWA'],
    color: '#3880FF',
    devPort: 5173,
    status: 'running',
    urlPath: 'ionic',
  },
];

const Home: React.FC = () => {
  const history = useHistory();

  const handleOpenApp = (app: AppInfo, embedded: boolean = false) => {
    if (app.devPort) {
      if (embedded) {
        history.push(`/webview/${app.urlPath}`);
      } else {
        window.open(`http://localhost:${app.devPort}`, '_blank');
      }
    } else {
      alert(`${app.name} еще не запущен. Запустите его вручную и обновите страницу.`);
    }
  };

  const getPlatformIcon = (platform: string) => {
    switch (platform) {
      case 'iOS':
        return logoApple;
      case 'Android':
        return logoAndroid;
      case 'Web':
      case 'PWA':
        return globeOutline;
      case 'Desktop':
      case 'Windows':
      case 'macOS':
        return desktopOutline;
      default:
        return rocketOutline;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'running':
        return 'success';
      case 'building':
        return 'warning';
      default:
        return 'medium';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'running':
        return 'Запущено';
      case 'building':
        return 'Сборка';
      default:
        return 'Остановлено';
    }
  };

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar color="primary">
          <IonTitle>White Broker - Launcher</IonTitle>
        </IonToolbar>
      </IonHeader>

      <IonContent fullscreen className="home-content">
        <div className="hero-section">
          <h1>🚀 White Broker Apps</h1>
          <p>Выберите приложение для запуска</p>
        </div>

        <IonGrid>
          <IonRow>
            {apps.map((app, index) => (
              <IonCol size="12" sizeMd="6" sizeLg="4" key={index}>
                <IonCard className="app-card">
                  <div
                    className="card-header-bg"
                    style={{ backgroundColor: app.color }}
                  >
                    <img
                      src={app.icon}
                      alt={app.name}
                      className="app-icon"
                      onError={(e) => {
                        e.currentTarget.src = 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><text y="50" font-size="50">📱</text></svg>';
                      }}
                    />
                  </div>

                  <IonCardHeader>
                    <IonCardTitle>{app.name}</IonCardTitle>
                    <IonCardSubtitle>{app.framework}</IonCardSubtitle>
                  </IonCardHeader>

                  <IonCardContent>
                    <p className="app-description">{app.description}</p>

                    <div className="platforms-section">
                      <strong>Платформы:</strong>
                      <div className="platform-chips">
                        {app.platforms.map((platform, idx) => (
                          <IonChip key={idx} color="primary" outline>
                            <IonIcon icon={getPlatformIcon(platform)} />
                            <IonLabel>{platform}</IonLabel>
                          </IonChip>
                        ))}
                      </div>
                    </div>

                    <div className="status-section">
                      <IonChip color={getStatusColor(app.status)}>
                        <IonLabel>{getStatusText(app.status)}</IonLabel>
                      </IonChip>
                      {app.devPort && (
                        <IonChip outline>
                          <IonLabel>Port: {app.devPort}</IonLabel>
                        </IonChip>
                      )}
                    </div>

                    <div className="button-group">
                      <IonButton
                        expand="block"
                        onClick={() => handleOpenApp(app, true)}
                        disabled={app.status !== 'running'}
                        color={app.status === 'running' ? 'primary' : 'medium'}
                      >
                        <IonIcon slot="start" icon={phonePortraitOutline} />
                        {app.status === 'running' ? 'Открыть встроенно' : 'Не запущено'}
                      </IonButton>

                      <IonButton
                        expand="block"
                        fill="outline"
                        onClick={() => handleOpenApp(app, false)}
                        disabled={app.status !== 'running'}
                        color="secondary"
                      >
                        <IonIcon slot="start" icon={openOutline} />
                        Открыть в новой вкладке
                      </IonButton>
                    </div>
                  </IonCardContent>
                </IonCard>
              </IonCol>
            ))}
          </IonRow>
        </IonGrid>

        <div className="info-section">
          <IonCard>
            <IonCardHeader>
              <IonCardTitle>ℹ️ Информация</IonCardTitle>
            </IonCardHeader>
            <IonCardContent>
              <p>
                <strong>Как запустить приложения:</strong>
              </p>
              <ul>
                <li>
                  <strong>Flutter:</strong> <code>cd whitebroker_app && flutter run -d chrome --web-port=8080</code>
                </li>
                <li>
                  <strong>.NET MAUI:</strong> <code>cd whitebroker_maui && dotnet run</code>
                </li>
                <li>
                  <strong>Ionic React:</strong> <code>cd whitebroker_ionic/white-broker && npm run dev</code>
                </li>
              </ul>
              <p className="note">
                💡 После запуска приложения обновите эту страницу, чтобы кнопки "Открыть" стали активными.
              </p>
              <p className="note">
                📱 "Открыть встроенно" - откроет приложение внутри Ionic в iframe<br/>
                🔗 "Открыть в новой вкладке" - откроет приложение в отдельной вкладке браузера
              </p>
            </IonCardContent>
          </IonCard>
        </div>
      </IonContent>
    </IonPage>
  );
};

export default Home;

