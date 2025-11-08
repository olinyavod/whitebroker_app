import {
  IonContent,
  IonHeader,
  IonPage,
  IonTitle,
  IonToolbar,
  IonButtons,
  IonBackButton,
  IonLoading,
} from '@ionic/react';
import { useState } from 'react';
import { useParams } from 'react-router';
import './WebViewPage.css';

interface WebViewParams {
  appName: string;
}

const appUrls: Record<string, string> = {
  flutter: 'http://localhost:8080',
  maui: 'http://localhost:5000',
  ionic: 'http://localhost:5173',
};

const WebViewPage: React.FC = () => {
  const { appName } = useParams<WebViewParams>();
  const [loading, setLoading] = useState(true);
  const url = appUrls[appName] || 'http://localhost:5173';

  const handleIframeLoad = () => {
    setLoading(false);
  };

  const handleIframeError = () => {
    setLoading(false);
  };

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar color="primary">
          <IonButtons slot="start">
            <IonBackButton defaultHref="/home" />
          </IonButtons>
          <IonTitle>
            {appName.charAt(0).toUpperCase() + appName.slice(1)} App
          </IonTitle>
        </IonToolbar>
      </IonHeader>

      <IonContent fullscreen className="webview-content">
        <IonLoading isOpen={loading} message="Загрузка приложения..." />

        <iframe
          src={url}
          className="app-iframe"
          onLoad={handleIframeLoad}
          onError={handleIframeError}
          title={`${appName} App`}
          allow="camera; microphone; geolocation; fullscreen"
        />

        {!loading && (
          <div className="iframe-overlay">
            <p>
              Если приложение не загрузилось, убедитесь что dev-сервер запущен на{' '}
              <a href={url} target="_blank" rel="noopener noreferrer">
                {url}
              </a>
            </p>
          </div>
        )}
      </IonContent>
    </IonPage>
  );
};

export default WebViewPage;










