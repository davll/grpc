#region Copyright notice and license

// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Grpc.Core.Logging;
using Grpc.Core.Utils;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Provides access to all native methods provided by <c>NativeExtension</c>.
    /// An extra level of indirection is added to P/Invoke calls to allow intelligent loading
    /// of the right configuration of the native extension based on current platform, architecture etc.
    /// </summary>
    internal class NativeMethods
    {
        #region Native methods

        public readonly Delegates.grpcsharp_init_delegate grpcsharp_init;
        public readonly Delegates.grpcsharp_shutdown_delegate grpcsharp_shutdown;
        public readonly Delegates.grpcsharp_version_string_delegate grpcsharp_version_string;

        public readonly Delegates.grpcsharp_batch_context_create_delegate grpcsharp_batch_context_create;
        public readonly Delegates.grpcsharp_batch_context_recv_initial_metadata_delegate grpcsharp_batch_context_recv_initial_metadata;
        public readonly Delegates.grpcsharp_batch_context_recv_message_length_delegate grpcsharp_batch_context_recv_message_length;
        public readonly Delegates.grpcsharp_batch_context_recv_message_to_buffer_delegate grpcsharp_batch_context_recv_message_to_buffer;
        public readonly Delegates.grpcsharp_batch_context_recv_status_on_client_status_delegate grpcsharp_batch_context_recv_status_on_client_status;
        public readonly Delegates.grpcsharp_batch_context_recv_status_on_client_details_delegate grpcsharp_batch_context_recv_status_on_client_details;
        public readonly Delegates.grpcsharp_batch_context_recv_status_on_client_trailing_metadata_delegate grpcsharp_batch_context_recv_status_on_client_trailing_metadata;
        public readonly Delegates.grpcsharp_batch_context_recv_close_on_server_cancelled_delegate grpcsharp_batch_context_recv_close_on_server_cancelled;
        public readonly Delegates.grpcsharp_batch_context_destroy_delegate grpcsharp_batch_context_destroy;

        public readonly Delegates.grpcsharp_request_call_context_create_delegate grpcsharp_request_call_context_create;
        public readonly Delegates.grpcsharp_request_call_context_call_delegate grpcsharp_request_call_context_call;
        public readonly Delegates.grpcsharp_request_call_context_method_delegate grpcsharp_request_call_context_method;
        public readonly Delegates.grpcsharp_request_call_context_host_delegate grpcsharp_request_call_context_host;
        public readonly Delegates.grpcsharp_request_call_context_deadline_delegate grpcsharp_request_call_context_deadline;
        public readonly Delegates.grpcsharp_request_call_context_request_metadata_delegate grpcsharp_request_call_context_request_metadata;
        public readonly Delegates.grpcsharp_request_call_context_destroy_delegate grpcsharp_request_call_context_destroy;

        public readonly Delegates.grpcsharp_composite_call_credentials_create_delegate grpcsharp_composite_call_credentials_create;
        public readonly Delegates.grpcsharp_call_credentials_release_delegate grpcsharp_call_credentials_release;

        public readonly Delegates.grpcsharp_call_cancel_delegate grpcsharp_call_cancel;
        public readonly Delegates.grpcsharp_call_cancel_with_status_delegate grpcsharp_call_cancel_with_status;
        public readonly Delegates.grpcsharp_call_start_unary_delegate grpcsharp_call_start_unary;
        public readonly Delegates.grpcsharp_call_start_client_streaming_delegate grpcsharp_call_start_client_streaming;
        public readonly Delegates.grpcsharp_call_start_server_streaming_delegate grpcsharp_call_start_server_streaming;
        public readonly Delegates.grpcsharp_call_start_duplex_streaming_delegate grpcsharp_call_start_duplex_streaming;
        public readonly Delegates.grpcsharp_call_send_message_delegate grpcsharp_call_send_message;
        public readonly Delegates.grpcsharp_call_send_close_from_client_delegate grpcsharp_call_send_close_from_client;
        public readonly Delegates.grpcsharp_call_send_status_from_server_delegate grpcsharp_call_send_status_from_server;
        public readonly Delegates.grpcsharp_call_recv_message_delegate grpcsharp_call_recv_message;
        public readonly Delegates.grpcsharp_call_recv_initial_metadata_delegate grpcsharp_call_recv_initial_metadata;
        public readonly Delegates.grpcsharp_call_start_serverside_delegate grpcsharp_call_start_serverside;
        public readonly Delegates.grpcsharp_call_send_initial_metadata_delegate grpcsharp_call_send_initial_metadata;
        public readonly Delegates.grpcsharp_call_set_credentials_delegate grpcsharp_call_set_credentials;
        public readonly Delegates.grpcsharp_call_get_peer_delegate grpcsharp_call_get_peer;
        public readonly Delegates.grpcsharp_call_destroy_delegate grpcsharp_call_destroy;

        public readonly Delegates.grpcsharp_channel_args_create_delegate grpcsharp_channel_args_create;
        public readonly Delegates.grpcsharp_channel_args_set_string_delegate grpcsharp_channel_args_set_string;
        public readonly Delegates.grpcsharp_channel_args_set_integer_delegate grpcsharp_channel_args_set_integer;
        public readonly Delegates.grpcsharp_channel_args_destroy_delegate grpcsharp_channel_args_destroy;

        public readonly Delegates.grpcsharp_override_default_ssl_roots grpcsharp_override_default_ssl_roots;
        public readonly Delegates.grpcsharp_ssl_credentials_create_delegate grpcsharp_ssl_credentials_create;
        public readonly Delegates.grpcsharp_composite_channel_credentials_create_delegate grpcsharp_composite_channel_credentials_create;
        public readonly Delegates.grpcsharp_channel_credentials_release_delegate grpcsharp_channel_credentials_release;

        public readonly Delegates.grpcsharp_insecure_channel_create_delegate grpcsharp_insecure_channel_create;
        public readonly Delegates.grpcsharp_secure_channel_create_delegate grpcsharp_secure_channel_create;
        public readonly Delegates.grpcsharp_channel_create_call_delegate grpcsharp_channel_create_call;
        public readonly Delegates.grpcsharp_channel_check_connectivity_state_delegate grpcsharp_channel_check_connectivity_state;
        public readonly Delegates.grpcsharp_channel_watch_connectivity_state_delegate grpcsharp_channel_watch_connectivity_state;
        public readonly Delegates.grpcsharp_channel_get_target_delegate grpcsharp_channel_get_target;
        public readonly Delegates.grpcsharp_channel_destroy_delegate grpcsharp_channel_destroy;

        public readonly Delegates.grpcsharp_sizeof_grpc_event_delegate grpcsharp_sizeof_grpc_event;

        public readonly Delegates.grpcsharp_completion_queue_create_async_delegate grpcsharp_completion_queue_create_async;
        public readonly Delegates.grpcsharp_completion_queue_create_sync_delegate grpcsharp_completion_queue_create_sync;
        public readonly Delegates.grpcsharp_completion_queue_shutdown_delegate grpcsharp_completion_queue_shutdown;
        public readonly Delegates.grpcsharp_completion_queue_next_delegate grpcsharp_completion_queue_next;
        public readonly Delegates.grpcsharp_completion_queue_pluck_delegate grpcsharp_completion_queue_pluck;
        public readonly Delegates.grpcsharp_completion_queue_destroy_delegate grpcsharp_completion_queue_destroy;

        public readonly Delegates.gprsharp_free_delegate gprsharp_free;

        public readonly Delegates.grpcsharp_metadata_array_create_delegate grpcsharp_metadata_array_create;
        public readonly Delegates.grpcsharp_metadata_array_add_delegate grpcsharp_metadata_array_add;
        public readonly Delegates.grpcsharp_metadata_array_count_delegate grpcsharp_metadata_array_count;
        public readonly Delegates.grpcsharp_metadata_array_get_key_delegate grpcsharp_metadata_array_get_key;
        public readonly Delegates.grpcsharp_metadata_array_get_value_delegate grpcsharp_metadata_array_get_value;
        public readonly Delegates.grpcsharp_metadata_array_destroy_full_delegate grpcsharp_metadata_array_destroy_full;

        public readonly Delegates.grpcsharp_redirect_log_delegate grpcsharp_redirect_log;

        public readonly Delegates.grpcsharp_metadata_credentials_create_from_plugin_delegate grpcsharp_metadata_credentials_create_from_plugin;
        public readonly Delegates.grpcsharp_metadata_credentials_notify_from_plugin_delegate grpcsharp_metadata_credentials_notify_from_plugin;

        public readonly Delegates.grpcsharp_ssl_server_credentials_create_delegate grpcsharp_ssl_server_credentials_create;
        public readonly Delegates.grpcsharp_server_credentials_release_delegate grpcsharp_server_credentials_release;

        public readonly Delegates.grpcsharp_server_create_delegate grpcsharp_server_create;
        public readonly Delegates.grpcsharp_server_register_completion_queue_delegate grpcsharp_server_register_completion_queue;
        public readonly Delegates.grpcsharp_server_add_insecure_http2_port_delegate grpcsharp_server_add_insecure_http2_port;
        public readonly Delegates.grpcsharp_server_add_secure_http2_port_delegate grpcsharp_server_add_secure_http2_port;
        public readonly Delegates.grpcsharp_server_start_delegate grpcsharp_server_start;
        public readonly Delegates.grpcsharp_server_request_call_delegate grpcsharp_server_request_call;
        public readonly Delegates.grpcsharp_server_cancel_all_calls_delegate grpcsharp_server_cancel_all_calls;
        public readonly Delegates.grpcsharp_server_shutdown_and_notify_callback_delegate grpcsharp_server_shutdown_and_notify_callback;
        public readonly Delegates.grpcsharp_server_destroy_delegate grpcsharp_server_destroy;

        public readonly Delegates.grpcsharp_call_auth_context_delegate grpcsharp_call_auth_context;
        public readonly Delegates.grpcsharp_auth_context_peer_identity_property_name_delegate grpcsharp_auth_context_peer_identity_property_name;
        public readonly Delegates.grpcsharp_auth_context_property_iterator_delegate grpcsharp_auth_context_property_iterator;
        public readonly Delegates.grpcsharp_auth_property_iterator_next_delegate grpcsharp_auth_property_iterator_next;
        public readonly Delegates.grpcsharp_auth_context_release_delegate grpcsharp_auth_context_release;

        public readonly Delegates.gprsharp_now_delegate gprsharp_now;
        public readonly Delegates.gprsharp_inf_future_delegate gprsharp_inf_future;
        public readonly Delegates.gprsharp_inf_past_delegate gprsharp_inf_past;
        public readonly Delegates.gprsharp_convert_clock_type_delegate gprsharp_convert_clock_type;
        public readonly Delegates.gprsharp_sizeof_timespec_delegate gprsharp_sizeof_timespec;

        public readonly Delegates.grpcsharp_test_callback_delegate grpcsharp_test_callback;
        public readonly Delegates.grpcsharp_test_nop_delegate grpcsharp_test_nop;

        public readonly Delegates.grpcsharp_test_override_method_delegate grpcsharp_test_override_method;

        #endregion

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
        public NativeMethods()
        {
            this.grpcsharp_init = NativeCalls.grpcsharp_init;
            this.grpcsharp_shutdown = NativeCalls.grpcsharp_shutdown;
            this.grpcsharp_version_string = NativeCalls.grpcsharp_version_string;

            this.grpcsharp_batch_context_create = NativeCalls.grpcsharp_batch_context_create;
            this.grpcsharp_batch_context_recv_initial_metadata = NativeCalls.grpcsharp_batch_context_recv_initial_metadata;
            this.grpcsharp_batch_context_recv_message_length = NativeCalls.grpcsharp_batch_context_recv_message_length;
            this.grpcsharp_batch_context_recv_message_to_buffer = NativeCalls.grpcsharp_batch_context_recv_message_to_buffer;
            this.grpcsharp_batch_context_recv_status_on_client_status = NativeCalls.grpcsharp_batch_context_recv_status_on_client_status;
            this.grpcsharp_batch_context_recv_status_on_client_details = NativeCalls.grpcsharp_batch_context_recv_status_on_client_details;
            this.grpcsharp_batch_context_recv_status_on_client_trailing_metadata = NativeCalls.grpcsharp_batch_context_recv_status_on_client_trailing_metadata;
            this.grpcsharp_batch_context_recv_close_on_server_cancelled = NativeCalls.grpcsharp_batch_context_recv_close_on_server_cancelled;
            this.grpcsharp_batch_context_destroy = NativeCalls.grpcsharp_batch_context_destroy;

            this.grpcsharp_request_call_context_create = NativeCalls.grpcsharp_request_call_context_create;
            this.grpcsharp_request_call_context_call = NativeCalls.grpcsharp_request_call_context_call;
            this.grpcsharp_request_call_context_method = NativeCalls.grpcsharp_request_call_context_method;
            this.grpcsharp_request_call_context_host = NativeCalls.grpcsharp_request_call_context_host;
            this.grpcsharp_request_call_context_deadline = NativeCalls.grpcsharp_request_call_context_deadline;
            this.grpcsharp_request_call_context_request_metadata = NativeCalls.grpcsharp_request_call_context_request_metadata;
            this.grpcsharp_request_call_context_destroy = NativeCalls.grpcsharp_request_call_context_destroy;

            this.grpcsharp_composite_call_credentials_create = NativeCalls.grpcsharp_composite_call_credentials_create;
            this.grpcsharp_call_credentials_release = NativeCalls.grpcsharp_call_credentials_release;

            this.grpcsharp_call_cancel = NativeCalls.grpcsharp_call_cancel;
            this.grpcsharp_call_cancel_with_status = NativeCalls.grpcsharp_call_cancel_with_status;
            this.grpcsharp_call_start_unary = NativeCalls.grpcsharp_call_start_unary;
            this.grpcsharp_call_start_client_streaming = NativeCalls.grpcsharp_call_start_client_streaming;
            this.grpcsharp_call_start_server_streaming = NativeCalls.grpcsharp_call_start_server_streaming;
            this.grpcsharp_call_start_duplex_streaming = NativeCalls.grpcsharp_call_start_duplex_streaming;
            this.grpcsharp_call_send_message = NativeCalls.grpcsharp_call_send_message;
            this.grpcsharp_call_send_close_from_client = NativeCalls.grpcsharp_call_send_close_from_client;
            this.grpcsharp_call_send_status_from_server = NativeCalls.grpcsharp_call_send_status_from_server;
            this.grpcsharp_call_recv_message = NativeCalls.grpcsharp_call_recv_message;
            this.grpcsharp_call_recv_initial_metadata = NativeCalls.grpcsharp_call_recv_initial_metadata;
            this.grpcsharp_call_start_serverside = NativeCalls.grpcsharp_call_start_serverside;
            this.grpcsharp_call_send_initial_metadata = NativeCalls.grpcsharp_call_send_initial_metadata;
            this.grpcsharp_call_set_credentials = NativeCalls.grpcsharp_call_set_credentials;
            this.grpcsharp_call_get_peer = NativeCalls.grpcsharp_call_get_peer;
            this.grpcsharp_call_destroy = NativeCalls.grpcsharp_call_destroy;

            this.grpcsharp_channel_args_create = NativeCalls.grpcsharp_channel_args_create;
            this.grpcsharp_channel_args_set_string = NativeCalls.grpcsharp_channel_args_set_string;
            this.grpcsharp_channel_args_set_integer = NativeCalls.grpcsharp_channel_args_set_integer;
            this.grpcsharp_channel_args_destroy = NativeCalls.grpcsharp_channel_args_destroy;

            this.grpcsharp_override_default_ssl_roots = NativeCalls.grpcsharp_override_default_ssl_roots;
            this.grpcsharp_ssl_credentials_create = NativeCalls.grpcsharp_ssl_credentials_create;
            this.grpcsharp_composite_channel_credentials_create = NativeCalls.grpcsharp_composite_channel_credentials_create;
            this.grpcsharp_channel_credentials_release = NativeCalls.grpcsharp_channel_credentials_release;

            this.grpcsharp_insecure_channel_create = NativeCalls.grpcsharp_insecure_channel_create;
            this.grpcsharp_secure_channel_create = NativeCalls.grpcsharp_secure_channel_create;
            this.grpcsharp_channel_create_call = NativeCalls.grpcsharp_channel_create_call;
            this.grpcsharp_channel_check_connectivity_state = NativeCalls.grpcsharp_channel_check_connectivity_state;
            this.grpcsharp_channel_watch_connectivity_state = NativeCalls.grpcsharp_channel_watch_connectivity_state;
            this.grpcsharp_channel_get_target = NativeCalls.grpcsharp_channel_get_target;
            this.grpcsharp_channel_destroy = NativeCalls.grpcsharp_channel_destroy;

            this.grpcsharp_sizeof_grpc_event = NativeCalls.grpcsharp_sizeof_grpc_event;

            this.grpcsharp_completion_queue_create_async = NativeCalls.grpcsharp_completion_queue_create_async;
            this.grpcsharp_completion_queue_create_sync = NativeCalls.grpcsharp_completion_queue_create_sync;
            this.grpcsharp_completion_queue_shutdown = NativeCalls.grpcsharp_completion_queue_shutdown;
            this.grpcsharp_completion_queue_next = NativeCalls.grpcsharp_completion_queue_next;
            this.grpcsharp_completion_queue_pluck = NativeCalls.grpcsharp_completion_queue_pluck;
            this.grpcsharp_completion_queue_destroy = NativeCalls.grpcsharp_completion_queue_destroy;

            this.gprsharp_free = NativeCalls.gprsharp_free;

            this.grpcsharp_metadata_array_create = NativeCalls.grpcsharp_metadata_array_create;
            this.grpcsharp_metadata_array_add = NativeCalls.grpcsharp_metadata_array_add;
            this.grpcsharp_metadata_array_count = NativeCalls.grpcsharp_metadata_array_count;
            this.grpcsharp_metadata_array_get_key = NativeCalls.grpcsharp_metadata_array_get_key;
            this.grpcsharp_metadata_array_get_value = NativeCalls.grpcsharp_metadata_array_get_value;
            this.grpcsharp_metadata_array_destroy_full = NativeCalls.grpcsharp_metadata_array_destroy_full;

            this.grpcsharp_redirect_log = NativeCalls.grpcsharp_redirect_log;

            this.grpcsharp_metadata_credentials_create_from_plugin = NativeCalls.grpcsharp_metadata_credentials_create_from_plugin;
            this.grpcsharp_metadata_credentials_notify_from_plugin = NativeCalls.grpcsharp_metadata_credentials_notify_from_plugin;

            this.grpcsharp_ssl_server_credentials_create = NativeCalls.grpcsharp_ssl_server_credentials_create;
            this.grpcsharp_server_credentials_release = NativeCalls.grpcsharp_server_credentials_release;

            this.grpcsharp_server_create = NativeCalls.grpcsharp_server_create;
            this.grpcsharp_server_register_completion_queue = NativeCalls.grpcsharp_server_register_completion_queue;
            this.grpcsharp_server_add_insecure_http2_port = NativeCalls.grpcsharp_server_add_insecure_http2_port;
            this.grpcsharp_server_add_secure_http2_port = NativeCalls.grpcsharp_server_add_secure_http2_port;
            this.grpcsharp_server_start = NativeCalls.grpcsharp_server_start;
            this.grpcsharp_server_request_call = NativeCalls.grpcsharp_server_request_call;
            this.grpcsharp_server_cancel_all_calls = NativeCalls.grpcsharp_server_cancel_all_calls;
            this.grpcsharp_server_shutdown_and_notify_callback = NativeCalls.grpcsharp_server_shutdown_and_notify_callback;
            this.grpcsharp_server_destroy = NativeCalls.grpcsharp_server_destroy;

            this.grpcsharp_call_auth_context = NativeCalls.grpcsharp_call_auth_context;
            this.grpcsharp_auth_context_peer_identity_property_name = NativeCalls.grpcsharp_auth_context_peer_identity_property_name;
            this.grpcsharp_auth_context_property_iterator = NativeCalls.grpcsharp_auth_context_property_iterator;
            this.grpcsharp_auth_property_iterator_next = NativeCalls.grpcsharp_auth_property_iterator_next;
            this.grpcsharp_auth_context_release = NativeCalls.grpcsharp_auth_context_release;

            this.gprsharp_now = NativeCalls.gprsharp_now;
            this.gprsharp_inf_future = NativeCalls.gprsharp_inf_future;
            this.gprsharp_inf_past = NativeCalls.gprsharp_inf_past;
            this.gprsharp_convert_clock_type = NativeCalls.gprsharp_convert_clock_type;
            this.gprsharp_sizeof_timespec = NativeCalls.gprsharp_sizeof_timespec;

            this.grpcsharp_test_callback = NativeCalls.grpcsharp_test_callback;
            this.grpcsharp_test_nop = NativeCalls.grpcsharp_test_nop;
            this.grpcsharp_test_override_method = NativeCalls.grpcsharp_test_override_method;
        }
#else
        public NativeMethods(UnmanagedLibrary library)
        {
            this.grpcsharp_init = GetMethodDelegate<Delegates.grpcsharp_init_delegate>(library);
            this.grpcsharp_shutdown = GetMethodDelegate<Delegates.grpcsharp_shutdown_delegate>(library);
            this.grpcsharp_version_string = GetMethodDelegate<Delegates.grpcsharp_version_string_delegate>(library);

            this.grpcsharp_batch_context_create = GetMethodDelegate<Delegates.grpcsharp_batch_context_create_delegate>(library);
            this.grpcsharp_batch_context_recv_initial_metadata = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_initial_metadata_delegate>(library);
            this.grpcsharp_batch_context_recv_message_length = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_message_length_delegate>(library);
            this.grpcsharp_batch_context_recv_message_to_buffer = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_message_to_buffer_delegate>(library);
            this.grpcsharp_batch_context_recv_status_on_client_status = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_status_on_client_status_delegate>(library);
            this.grpcsharp_batch_context_recv_status_on_client_details = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_status_on_client_details_delegate>(library);
            this.grpcsharp_batch_context_recv_status_on_client_trailing_metadata = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_status_on_client_trailing_metadata_delegate>(library);
            this.grpcsharp_batch_context_recv_close_on_server_cancelled = GetMethodDelegate<Delegates.grpcsharp_batch_context_recv_close_on_server_cancelled_delegate>(library);
            this.grpcsharp_batch_context_destroy = GetMethodDelegate<Delegates.grpcsharp_batch_context_destroy_delegate>(library);

            this.grpcsharp_request_call_context_create = GetMethodDelegate<Delegates.grpcsharp_request_call_context_create_delegate>(library);
            this.grpcsharp_request_call_context_call = GetMethodDelegate<Delegates.grpcsharp_request_call_context_call_delegate>(library);
            this.grpcsharp_request_call_context_method = GetMethodDelegate<Delegates.grpcsharp_request_call_context_method_delegate>(library);
            this.grpcsharp_request_call_context_host = GetMethodDelegate<Delegates.grpcsharp_request_call_context_host_delegate>(library);
            this.grpcsharp_request_call_context_deadline = GetMethodDelegate<Delegates.grpcsharp_request_call_context_deadline_delegate>(library);
            this.grpcsharp_request_call_context_request_metadata = GetMethodDelegate<Delegates.grpcsharp_request_call_context_request_metadata_delegate>(library);
            this.grpcsharp_request_call_context_destroy = GetMethodDelegate<Delegates.grpcsharp_request_call_context_destroy_delegate>(library);

            this.grpcsharp_composite_call_credentials_create = GetMethodDelegate<Delegates.grpcsharp_composite_call_credentials_create_delegate>(library);
            this.grpcsharp_call_credentials_release = GetMethodDelegate<Delegates.grpcsharp_call_credentials_release_delegate>(library);

            this.grpcsharp_call_cancel = GetMethodDelegate<Delegates.grpcsharp_call_cancel_delegate>(library);
            this.grpcsharp_call_cancel_with_status = GetMethodDelegate<Delegates.grpcsharp_call_cancel_with_status_delegate>(library);
            this.grpcsharp_call_start_unary = GetMethodDelegate<Delegates.grpcsharp_call_start_unary_delegate>(library);
            this.grpcsharp_call_start_client_streaming = GetMethodDelegate<Delegates.grpcsharp_call_start_client_streaming_delegate>(library);
            this.grpcsharp_call_start_server_streaming = GetMethodDelegate<Delegates.grpcsharp_call_start_server_streaming_delegate>(library);
            this.grpcsharp_call_start_duplex_streaming = GetMethodDelegate<Delegates.grpcsharp_call_start_duplex_streaming_delegate>(library);
            this.grpcsharp_call_send_message = GetMethodDelegate<Delegates.grpcsharp_call_send_message_delegate>(library);
            this.grpcsharp_call_send_close_from_client = GetMethodDelegate<Delegates.grpcsharp_call_send_close_from_client_delegate>(library);
            this.grpcsharp_call_send_status_from_server = GetMethodDelegate<Delegates.grpcsharp_call_send_status_from_server_delegate>(library);
            this.grpcsharp_call_recv_message = GetMethodDelegate<Delegates.grpcsharp_call_recv_message_delegate>(library);
            this.grpcsharp_call_recv_initial_metadata = GetMethodDelegate<Delegates.grpcsharp_call_recv_initial_metadata_delegate>(library);
            this.grpcsharp_call_start_serverside = GetMethodDelegate<Delegates.grpcsharp_call_start_serverside_delegate>(library);
            this.grpcsharp_call_send_initial_metadata = GetMethodDelegate<Delegates.grpcsharp_call_send_initial_metadata_delegate>(library);
            this.grpcsharp_call_set_credentials = GetMethodDelegate<Delegates.grpcsharp_call_set_credentials_delegate>(library);
            this.grpcsharp_call_get_peer = GetMethodDelegate<Delegates.grpcsharp_call_get_peer_delegate>(library);
            this.grpcsharp_call_destroy = GetMethodDelegate<Delegates.grpcsharp_call_destroy_delegate>(library);

            this.grpcsharp_channel_args_create = GetMethodDelegate<Delegates.grpcsharp_channel_args_create_delegate>(library);
            this.grpcsharp_channel_args_set_string = GetMethodDelegate<Delegates.grpcsharp_channel_args_set_string_delegate>(library);
            this.grpcsharp_channel_args_set_integer = GetMethodDelegate<Delegates.grpcsharp_channel_args_set_integer_delegate>(library);
            this.grpcsharp_channel_args_destroy = GetMethodDelegate<Delegates.grpcsharp_channel_args_destroy_delegate>(library);

            this.grpcsharp_override_default_ssl_roots = GetMethodDelegate<Delegates.grpcsharp_override_default_ssl_roots>(library);
            this.grpcsharp_ssl_credentials_create = GetMethodDelegate<Delegates.grpcsharp_ssl_credentials_create_delegate>(library);
            this.grpcsharp_composite_channel_credentials_create = GetMethodDelegate<Delegates.grpcsharp_composite_channel_credentials_create_delegate>(library);
            this.grpcsharp_channel_credentials_release = GetMethodDelegate<Delegates.grpcsharp_channel_credentials_release_delegate>(library);

            this.grpcsharp_insecure_channel_create = GetMethodDelegate<Delegates.grpcsharp_insecure_channel_create_delegate>(library);
            this.grpcsharp_secure_channel_create = GetMethodDelegate<Delegates.grpcsharp_secure_channel_create_delegate>(library);
            this.grpcsharp_channel_create_call = GetMethodDelegate<Delegates.grpcsharp_channel_create_call_delegate>(library);
            this.grpcsharp_channel_check_connectivity_state = GetMethodDelegate<Delegates.grpcsharp_channel_check_connectivity_state_delegate>(library);
            this.grpcsharp_channel_watch_connectivity_state = GetMethodDelegate<Delegates.grpcsharp_channel_watch_connectivity_state_delegate>(library);
            this.grpcsharp_channel_get_target = GetMethodDelegate<Delegates.grpcsharp_channel_get_target_delegate>(library);
            this.grpcsharp_channel_destroy = GetMethodDelegate<Delegates.grpcsharp_channel_destroy_delegate>(library);

            this.grpcsharp_sizeof_grpc_event = GetMethodDelegate<Delegates.grpcsharp_sizeof_grpc_event_delegate>(library);

            this.grpcsharp_completion_queue_create_async = GetMethodDelegate<Delegates.grpcsharp_completion_queue_create_async_delegate>(library);
            this.grpcsharp_completion_queue_create_sync = GetMethodDelegate<Delegates.grpcsharp_completion_queue_create_sync_delegate>(library);
            this.grpcsharp_completion_queue_shutdown = GetMethodDelegate<Delegates.grpcsharp_completion_queue_shutdown_delegate>(library);
            this.grpcsharp_completion_queue_next = GetMethodDelegate<Delegates.grpcsharp_completion_queue_next_delegate>(library);
            this.grpcsharp_completion_queue_pluck = GetMethodDelegate<Delegates.grpcsharp_completion_queue_pluck_delegate>(library);
            this.grpcsharp_completion_queue_destroy = GetMethodDelegate<Delegates.grpcsharp_completion_queue_destroy_delegate>(library);

            this.gprsharp_free = GetMethodDelegate<Delegates.gprsharp_free_delegate>(library);

            this.grpcsharp_metadata_array_create = GetMethodDelegate<Delegates.grpcsharp_metadata_array_create_delegate>(library);
            this.grpcsharp_metadata_array_add = GetMethodDelegate<Delegates.grpcsharp_metadata_array_add_delegate>(library);
            this.grpcsharp_metadata_array_count = GetMethodDelegate<Delegates.grpcsharp_metadata_array_count_delegate>(library);
            this.grpcsharp_metadata_array_get_key = GetMethodDelegate<Delegates.grpcsharp_metadata_array_get_key_delegate>(library);
            this.grpcsharp_metadata_array_get_value = GetMethodDelegate<Delegates.grpcsharp_metadata_array_get_value_delegate>(library);
            this.grpcsharp_metadata_array_destroy_full = GetMethodDelegate<Delegates.grpcsharp_metadata_array_destroy_full_delegate>(library);

            this.grpcsharp_redirect_log = GetMethodDelegate<Delegates.grpcsharp_redirect_log_delegate>(library);

            this.grpcsharp_metadata_credentials_create_from_plugin = GetMethodDelegate<Delegates.grpcsharp_metadata_credentials_create_from_plugin_delegate>(library);
            this.grpcsharp_metadata_credentials_notify_from_plugin = GetMethodDelegate<Delegates.grpcsharp_metadata_credentials_notify_from_plugin_delegate>(library);

            this.grpcsharp_ssl_server_credentials_create = GetMethodDelegate<Delegates.grpcsharp_ssl_server_credentials_create_delegate>(library);
            this.grpcsharp_server_credentials_release = GetMethodDelegate<Delegates.grpcsharp_server_credentials_release_delegate>(library);

            this.grpcsharp_server_create = GetMethodDelegate<Delegates.grpcsharp_server_create_delegate>(library);
            this.grpcsharp_server_register_completion_queue = GetMethodDelegate<Delegates.grpcsharp_server_register_completion_queue_delegate>(library);
            this.grpcsharp_server_add_insecure_http2_port = GetMethodDelegate<Delegates.grpcsharp_server_add_insecure_http2_port_delegate>(library);
            this.grpcsharp_server_add_secure_http2_port = GetMethodDelegate<Delegates.grpcsharp_server_add_secure_http2_port_delegate>(library);
            this.grpcsharp_server_start = GetMethodDelegate<Delegates.grpcsharp_server_start_delegate>(library);
            this.grpcsharp_server_request_call = GetMethodDelegate<Delegates.grpcsharp_server_request_call_delegate>(library);
            this.grpcsharp_server_cancel_all_calls = GetMethodDelegate<Delegates.grpcsharp_server_cancel_all_calls_delegate>(library);
            this.grpcsharp_server_shutdown_and_notify_callback = GetMethodDelegate<Delegates.grpcsharp_server_shutdown_and_notify_callback_delegate>(library);
            this.grpcsharp_server_destroy = GetMethodDelegate<Delegates.grpcsharp_server_destroy_delegate>(library);

            this.grpcsharp_call_auth_context = GetMethodDelegate<Delegates.grpcsharp_call_auth_context_delegate>(library);
            this.grpcsharp_auth_context_peer_identity_property_name = GetMethodDelegate<Delegates.grpcsharp_auth_context_peer_identity_property_name_delegate>(library);
            this.grpcsharp_auth_context_property_iterator = GetMethodDelegate<Delegates.grpcsharp_auth_context_property_iterator_delegate>(library);
            this.grpcsharp_auth_property_iterator_next = GetMethodDelegate<Delegates.grpcsharp_auth_property_iterator_next_delegate>(library);
            this.grpcsharp_auth_context_release = GetMethodDelegate<Delegates.grpcsharp_auth_context_release_delegate>(library);

            this.gprsharp_now = GetMethodDelegate<Delegates.gprsharp_now_delegate>(library);
            this.gprsharp_inf_future = GetMethodDelegate<Delegates.gprsharp_inf_future_delegate>(library);
            this.gprsharp_inf_past = GetMethodDelegate<Delegates.gprsharp_inf_past_delegate>(library);
            this.gprsharp_convert_clock_type = GetMethodDelegate<Delegates.gprsharp_convert_clock_type_delegate>(library);
            this.gprsharp_sizeof_timespec = GetMethodDelegate<Delegates.gprsharp_sizeof_timespec_delegate>(library);

            this.grpcsharp_test_callback = GetMethodDelegate<Delegates.grpcsharp_test_callback_delegate>(library);
            this.grpcsharp_test_nop = GetMethodDelegate<Delegates.grpcsharp_test_nop_delegate>(library);
            this.grpcsharp_test_override_method = GetMethodDelegate<Delegates.grpcsharp_test_override_method_delegate>(library);
        }
#endif

        /// <summary>
        /// Gets singleton instance of this class.
        /// </summary>
        public static NativeMethods Get()
        {
            return NativeExtension.Get().NativeMethods;
        }

#if !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
        static T GetMethodDelegate<T>(UnmanagedLibrary library)
            where T : class
        {
            var methodName = RemoveStringSuffix(typeof(T).Name, "_delegate");
            return library.GetNativeMethodDelegate<T>(methodName);
        }

        static string RemoveStringSuffix(string str, string toRemove)
        {
            if (!str.EndsWith(toRemove))
            {
                return str;
            }
            return str.Substring(0, str.Length - toRemove.Length);
        }
#endif

        /// <summary>
        /// Delegate types for all published native methods. Declared under inner class to prevent scope pollution.
        /// </summary>
        public class Delegates
        {
            public delegate void grpcsharp_init_delegate();
            public delegate void grpcsharp_shutdown_delegate();
            public delegate IntPtr grpcsharp_version_string_delegate();  // returns not-owned const char*

            public delegate BatchContextSafeHandle grpcsharp_batch_context_create_delegate();
            public delegate IntPtr grpcsharp_batch_context_recv_initial_metadata_delegate(BatchContextSafeHandle ctx);
            public delegate IntPtr grpcsharp_batch_context_recv_message_length_delegate(BatchContextSafeHandle ctx);
            public delegate void grpcsharp_batch_context_recv_message_to_buffer_delegate(BatchContextSafeHandle ctx, byte[] buffer, UIntPtr bufferLen);
            public delegate StatusCode grpcsharp_batch_context_recv_status_on_client_status_delegate(BatchContextSafeHandle ctx);
            public delegate IntPtr grpcsharp_batch_context_recv_status_on_client_details_delegate(BatchContextSafeHandle ctx, out UIntPtr detailsLength);
            public delegate IntPtr grpcsharp_batch_context_recv_status_on_client_trailing_metadata_delegate(BatchContextSafeHandle ctx);
            public delegate int grpcsharp_batch_context_recv_close_on_server_cancelled_delegate(BatchContextSafeHandle ctx);
            public delegate void grpcsharp_batch_context_destroy_delegate(IntPtr ctx);

            public delegate RequestCallContextSafeHandle grpcsharp_request_call_context_create_delegate();
            public delegate CallSafeHandle grpcsharp_request_call_context_call_delegate(RequestCallContextSafeHandle ctx);
            public delegate IntPtr grpcsharp_request_call_context_method_delegate(RequestCallContextSafeHandle ctx, out UIntPtr methodLength);
            public delegate IntPtr grpcsharp_request_call_context_host_delegate(RequestCallContextSafeHandle ctx, out UIntPtr hostLength);
            public delegate Timespec grpcsharp_request_call_context_deadline_delegate(RequestCallContextSafeHandle ctx);
            public delegate IntPtr grpcsharp_request_call_context_request_metadata_delegate(RequestCallContextSafeHandle ctx);
            public delegate void grpcsharp_request_call_context_destroy_delegate(IntPtr ctx);

            public delegate CallCredentialsSafeHandle grpcsharp_composite_call_credentials_create_delegate(CallCredentialsSafeHandle creds1, CallCredentialsSafeHandle creds2);
            public delegate void grpcsharp_call_credentials_release_delegate(IntPtr credentials);

            public delegate CallError grpcsharp_call_cancel_delegate(CallSafeHandle call);
            public delegate CallError grpcsharp_call_cancel_with_status_delegate(CallSafeHandle call, StatusCode status, string description);
            public delegate CallError grpcsharp_call_start_unary_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            public delegate CallError grpcsharp_call_start_client_streaming_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            public delegate CallError grpcsharp_call_start_server_streaming_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags,
                MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            public delegate CallError grpcsharp_call_start_duplex_streaming_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            public delegate CallError grpcsharp_call_send_message_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags, int sendEmptyInitialMetadata);
            public delegate CallError grpcsharp_call_send_close_from_client_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            public delegate CallError grpcsharp_call_send_status_from_server_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, StatusCode statusCode, byte[] statusMessage, UIntPtr statusMessageLen, MetadataArraySafeHandle metadataArray, int sendEmptyInitialMetadata,
                byte[] optionalSendBuffer, UIntPtr optionalSendBufferLen, WriteFlags writeFlags);
            public delegate CallError grpcsharp_call_recv_message_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            public delegate CallError grpcsharp_call_recv_initial_metadata_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            public delegate CallError grpcsharp_call_start_serverside_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            public delegate CallError grpcsharp_call_send_initial_metadata_delegate(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray);
            public delegate CallError grpcsharp_call_set_credentials_delegate(CallSafeHandle call, CallCredentialsSafeHandle credentials);
            public delegate CStringSafeHandle grpcsharp_call_get_peer_delegate(CallSafeHandle call);
            public delegate void grpcsharp_call_destroy_delegate(IntPtr call);

            public delegate ChannelArgsSafeHandle grpcsharp_channel_args_create_delegate(UIntPtr numArgs);
            public delegate void grpcsharp_channel_args_set_string_delegate(ChannelArgsSafeHandle args, UIntPtr index, string key, string value);
            public delegate void grpcsharp_channel_args_set_integer_delegate(ChannelArgsSafeHandle args, UIntPtr index, string key, int value);
            public delegate void grpcsharp_channel_args_destroy_delegate(IntPtr args);

            public delegate void grpcsharp_override_default_ssl_roots(string pemRootCerts);
            public delegate ChannelCredentialsSafeHandle grpcsharp_ssl_credentials_create_delegate(string pemRootCerts, string keyCertPairCertChain, string keyCertPairPrivateKey);
            public delegate ChannelCredentialsSafeHandle grpcsharp_composite_channel_credentials_create_delegate(ChannelCredentialsSafeHandle channelCreds, CallCredentialsSafeHandle callCreds);
            public delegate void grpcsharp_channel_credentials_release_delegate(IntPtr credentials);

            public delegate ChannelSafeHandle grpcsharp_insecure_channel_create_delegate(string target, ChannelArgsSafeHandle channelArgs);
            public delegate ChannelSafeHandle grpcsharp_secure_channel_create_delegate(ChannelCredentialsSafeHandle credentials, string target, ChannelArgsSafeHandle channelArgs);
            public delegate CallSafeHandle grpcsharp_channel_create_call_delegate(ChannelSafeHandle channel, CallSafeHandle parentCall, ContextPropagationFlags propagationMask, CompletionQueueSafeHandle cq, string method, string host, Timespec deadline);
            public delegate ChannelState grpcsharp_channel_check_connectivity_state_delegate(ChannelSafeHandle channel, int tryToConnect);
            public delegate void grpcsharp_channel_watch_connectivity_state_delegate(ChannelSafeHandle channel, ChannelState lastObservedState,
                Timespec deadline, CompletionQueueSafeHandle cq, BatchContextSafeHandle ctx);
            public delegate CStringSafeHandle grpcsharp_channel_get_target_delegate(ChannelSafeHandle call);
            public delegate void grpcsharp_channel_destroy_delegate(IntPtr channel);

            public delegate int grpcsharp_sizeof_grpc_event_delegate();

            public delegate CompletionQueueSafeHandle grpcsharp_completion_queue_create_async_delegate();
            public delegate CompletionQueueSafeHandle grpcsharp_completion_queue_create_sync_delegate();
            public delegate void grpcsharp_completion_queue_shutdown_delegate(CompletionQueueSafeHandle cq);
            public delegate CompletionQueueEvent grpcsharp_completion_queue_next_delegate(CompletionQueueSafeHandle cq);
            public delegate CompletionQueueEvent grpcsharp_completion_queue_pluck_delegate(CompletionQueueSafeHandle cq, IntPtr tag);
            public delegate void grpcsharp_completion_queue_destroy_delegate(IntPtr cq);

            public delegate void gprsharp_free_delegate(IntPtr ptr);

            public delegate MetadataArraySafeHandle grpcsharp_metadata_array_create_delegate(UIntPtr capacity);
            public delegate void grpcsharp_metadata_array_add_delegate(MetadataArraySafeHandle array, string key, byte[] value, UIntPtr valueLength);
            public delegate UIntPtr grpcsharp_metadata_array_count_delegate(IntPtr metadataArray);
            public delegate IntPtr grpcsharp_metadata_array_get_key_delegate(IntPtr metadataArray, UIntPtr index, out UIntPtr keyLength);
            public delegate IntPtr grpcsharp_metadata_array_get_value_delegate(IntPtr metadataArray, UIntPtr index, out UIntPtr valueLength);
            public delegate void grpcsharp_metadata_array_destroy_full_delegate(IntPtr array);

            public delegate void grpcsharp_redirect_log_delegate(GprLogDelegate callback);

            public delegate CallCredentialsSafeHandle grpcsharp_metadata_credentials_create_from_plugin_delegate(NativeMetadataInterceptor interceptor);
            public delegate void grpcsharp_metadata_credentials_notify_from_plugin_delegate(IntPtr callbackPtr, IntPtr userData, MetadataArraySafeHandle metadataArray, StatusCode statusCode, string errorDetails);

            public delegate ServerCredentialsSafeHandle grpcsharp_ssl_server_credentials_create_delegate(string pemRootCerts, string[] keyCertPairCertChainArray, string[] keyCertPairPrivateKeyArray, UIntPtr numKeyCertPairs, int forceClientAuth);
            public delegate void grpcsharp_server_credentials_release_delegate(IntPtr credentials);

            public delegate ServerSafeHandle grpcsharp_server_create_delegate(ChannelArgsSafeHandle args);
            public delegate void grpcsharp_server_register_completion_queue_delegate(ServerSafeHandle server, CompletionQueueSafeHandle cq);
            public delegate int grpcsharp_server_add_insecure_http2_port_delegate(ServerSafeHandle server, string addr);
            public delegate int grpcsharp_server_add_secure_http2_port_delegate(ServerSafeHandle server, string addr, ServerCredentialsSafeHandle creds);
            public delegate void grpcsharp_server_start_delegate(ServerSafeHandle server);
            public delegate CallError grpcsharp_server_request_call_delegate(ServerSafeHandle server, CompletionQueueSafeHandle cq, RequestCallContextSafeHandle ctx);
            public delegate void grpcsharp_server_cancel_all_calls_delegate(ServerSafeHandle server);
            public delegate void grpcsharp_server_shutdown_and_notify_callback_delegate(ServerSafeHandle server, CompletionQueueSafeHandle cq, BatchContextSafeHandle ctx);
            public delegate void grpcsharp_server_destroy_delegate(IntPtr server);

            public delegate AuthContextSafeHandle grpcsharp_call_auth_context_delegate(CallSafeHandle call);
            public delegate IntPtr grpcsharp_auth_context_peer_identity_property_name_delegate(AuthContextSafeHandle authContext);  // returns const char*
            public delegate AuthContextSafeHandle.NativeAuthPropertyIterator grpcsharp_auth_context_property_iterator_delegate(AuthContextSafeHandle authContext);
            public delegate IntPtr grpcsharp_auth_property_iterator_next_delegate(ref AuthContextSafeHandle.NativeAuthPropertyIterator iterator);  // returns const auth_property*
            public delegate void grpcsharp_auth_context_release_delegate(IntPtr authContext);

            public delegate Timespec gprsharp_now_delegate(ClockType clockType);
            public delegate Timespec gprsharp_inf_future_delegate(ClockType clockType);
            public delegate Timespec gprsharp_inf_past_delegate(ClockType clockType);

            public delegate Timespec gprsharp_convert_clock_type_delegate(Timespec t, ClockType targetClock);
            public delegate int gprsharp_sizeof_timespec_delegate();

            public delegate CallError grpcsharp_test_callback_delegate([MarshalAs(UnmanagedType.FunctionPtr)] OpCompletionDelegate callback);
            public delegate IntPtr grpcsharp_test_nop_delegate(IntPtr ptr);
            public delegate void grpcsharp_test_override_method_delegate(string methodName, string variant);
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
        private static class NativeCalls {
#if UNITY_EDITOR		
            private const string pluginName = "grpc_csharp_ext";		
#elif UNITY_IOS || UNITY_TVOS || UNITY_WEBGL		
            public const string pluginName = "__Internal";		
#else		
            public const string pluginName = "grpc_csharp_ext";		
#endif

            [DllImport(pluginName)]
            public static extern void grpcsharp_init();
            [DllImport(pluginName)]
            public static extern void grpcsharp_shutdown();
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_version_string();
            [DllImport(pluginName)]
            public static extern BatchContextSafeHandle grpcsharp_batch_context_create();
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_batch_context_recv_initial_metadata(BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_batch_context_recv_message_length(BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern void grpcsharp_batch_context_recv_message_to_buffer(BatchContextSafeHandle ctx, byte[] buffer, UIntPtr bufferLen);
            [DllImport(pluginName)]
            public static extern StatusCode grpcsharp_batch_context_recv_status_on_client_status(BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_batch_context_recv_status_on_client_details(BatchContextSafeHandle ctx, out UIntPtr detailsLength);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_batch_context_recv_status_on_client_trailing_metadata(BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern int grpcsharp_batch_context_recv_close_on_server_cancelled(BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern void grpcsharp_batch_context_destroy(IntPtr ctx);
            [DllImport(pluginName)]
            public static extern RequestCallContextSafeHandle grpcsharp_request_call_context_create();
            [DllImport(pluginName)]
            public static extern CallSafeHandle grpcsharp_request_call_context_call(RequestCallContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_request_call_context_method(RequestCallContextSafeHandle ctx, out UIntPtr methodLength);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_request_call_context_host(RequestCallContextSafeHandle ctx, out UIntPtr hostLength);
            [DllImport(pluginName)]
            public static extern Timespec grpcsharp_request_call_context_deadline(RequestCallContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_request_call_context_request_metadata(RequestCallContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern void grpcsharp_request_call_context_destroy(IntPtr ctx);
            [DllImport(pluginName)]
            public static extern CallCredentialsSafeHandle grpcsharp_composite_call_credentials_create(CallCredentialsSafeHandle creds1, CallCredentialsSafeHandle creds2);
            [DllImport(pluginName)]
            public static extern void grpcsharp_call_credentials_release(IntPtr credentials);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_cancel(CallSafeHandle call);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_cancel_with_status(CallSafeHandle call, StatusCode status, string description);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_start_unary(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_start_client_streaming(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_start_server_streaming(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags,
                MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_start_duplex_streaming(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray, CallFlags metadataFlags);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_send_message(CallSafeHandle call,
                BatchContextSafeHandle ctx, byte[] sendBuffer, UIntPtr sendBufferLen, WriteFlags writeFlags, int sendEmptyInitialMetadata);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_send_close_from_client(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_send_status_from_server(CallSafeHandle call,
                BatchContextSafeHandle ctx, StatusCode statusCode, byte[] statusMessage, UIntPtr statusMessageLen, MetadataArraySafeHandle metadataArray, int sendEmptyInitialMetadata,
                byte[] optionalSendBuffer, UIntPtr optionalSendBufferLen, WriteFlags writeFlags);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_recv_message(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_recv_initial_metadata(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_start_serverside(CallSafeHandle call,
                BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_send_initial_metadata(CallSafeHandle call,
                BatchContextSafeHandle ctx, MetadataArraySafeHandle metadataArray);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_call_set_credentials(CallSafeHandle call, CallCredentialsSafeHandle credentials);
            [DllImport(pluginName)]
            public static extern CStringSafeHandle grpcsharp_call_get_peer(CallSafeHandle call);
            [DllImport(pluginName)]
            public static extern void grpcsharp_call_destroy(IntPtr call);
            [DllImport(pluginName)]
            public static extern ChannelArgsSafeHandle grpcsharp_channel_args_create(UIntPtr numArgs);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_args_set_string(ChannelArgsSafeHandle args, UIntPtr index, string key, string value);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_args_set_integer(ChannelArgsSafeHandle args, UIntPtr index, string key, int value);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_args_destroy(IntPtr args);
            [DllImport(pluginName)]
            public static extern void grpcsharp_override_default_ssl_roots(string pemRootCerts);
            [DllImport(pluginName)]
            public static extern ChannelCredentialsSafeHandle grpcsharp_ssl_credentials_create(string pemRootCerts, string keyCertPairCertChain, string keyCertPairPrivateKey);
            [DllImport(pluginName)]
            public static extern ChannelCredentialsSafeHandle grpcsharp_composite_channel_credentials_create(ChannelCredentialsSafeHandle channelCreds, CallCredentialsSafeHandle callCreds);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_credentials_release(IntPtr credentials);
            [DllImport(pluginName)]
            public static extern ChannelSafeHandle grpcsharp_insecure_channel_create(string target, ChannelArgsSafeHandle channelArgs);
            [DllImport(pluginName)]
            public static extern ChannelSafeHandle grpcsharp_secure_channel_create(ChannelCredentialsSafeHandle credentials, string target, ChannelArgsSafeHandle channelArgs);
            [DllImport(pluginName)]
            public static extern CallSafeHandle grpcsharp_channel_create_call(ChannelSafeHandle channel, CallSafeHandle parentCall, ContextPropagationFlags propagationMask, CompletionQueueSafeHandle cq, string method, string host, Timespec deadline);
            [DllImport(pluginName)]
            public static extern ChannelState grpcsharp_channel_check_connectivity_state(ChannelSafeHandle channel, int tryToConnect);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_watch_connectivity_state(ChannelSafeHandle channel, ChannelState lastObservedState,
                Timespec deadline, CompletionQueueSafeHandle cq, BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern CStringSafeHandle grpcsharp_channel_get_target(ChannelSafeHandle call);
            [DllImport(pluginName)]
            public static extern void grpcsharp_channel_destroy(IntPtr channel);
            [DllImport(pluginName)]
            public static extern int grpcsharp_sizeof_grpc_event();
            [DllImport(pluginName)]
            public static extern CompletionQueueSafeHandle grpcsharp_completion_queue_create_async();
            [DllImport(pluginName)]
            public static extern CompletionQueueSafeHandle grpcsharp_completion_queue_create_sync();
            [DllImport(pluginName)]
            public static extern void grpcsharp_completion_queue_shutdown(CompletionQueueSafeHandle cq);
            [DllImport(pluginName)]
            public static extern CompletionQueueEvent grpcsharp_completion_queue_next(CompletionQueueSafeHandle cq);
            [DllImport(pluginName)]
            public static extern CompletionQueueEvent grpcsharp_completion_queue_pluck(CompletionQueueSafeHandle cq, IntPtr tag);
            [DllImport(pluginName)]
            public static extern void grpcsharp_completion_queue_destroy(IntPtr cq);
            [DllImport(pluginName)]
            public static extern void gprsharp_free(IntPtr ptr);
            [DllImport(pluginName)]
            public static extern MetadataArraySafeHandle grpcsharp_metadata_array_create(UIntPtr capacity);
            [DllImport(pluginName)]
            public static extern void grpcsharp_metadata_array_add(MetadataArraySafeHandle array, string key, byte[] value, UIntPtr valueLength);
            [DllImport(pluginName)]
            public static extern UIntPtr grpcsharp_metadata_array_count(IntPtr metadataArray);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_metadata_array_get_key(IntPtr metadataArray, UIntPtr index, out UIntPtr keyLength);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_metadata_array_get_value(IntPtr metadataArray, UIntPtr index, out UIntPtr valueLength);
            [DllImport(pluginName)]
            public static extern void grpcsharp_metadata_array_destroy_full(IntPtr array);
            [DllImport(pluginName)]
            public static extern void grpcsharp_redirect_log(GprLogDelegate callback);
            [DllImport(pluginName)]
            public static extern CallCredentialsSafeHandle grpcsharp_metadata_credentials_create_from_plugin(NativeMetadataInterceptor interceptor);
            [DllImport(pluginName)]
            public static extern void grpcsharp_metadata_credentials_notify_from_plugin(IntPtr callbackPtr, IntPtr userData, MetadataArraySafeHandle metadataArray, StatusCode statusCode, string errorDetails);
            [DllImport(pluginName)]
            public static extern ServerCredentialsSafeHandle grpcsharp_ssl_server_credentials_create(string pemRootCerts, string[] keyCertPairCertChainArray, string[] keyCertPairPrivateKeyArray, UIntPtr numKeyCertPairs, int forceClientAuth);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_credentials_release(IntPtr credentials);
            [DllImport(pluginName)]
            public static extern ServerSafeHandle grpcsharp_server_create(ChannelArgsSafeHandle args);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_register_completion_queue(ServerSafeHandle server, CompletionQueueSafeHandle cq);
            [DllImport(pluginName)]
            public static extern int grpcsharp_server_add_insecure_http2_port(ServerSafeHandle server, string addr);
            [DllImport(pluginName)]
            public static extern int grpcsharp_server_add_secure_http2_port(ServerSafeHandle server, string addr, ServerCredentialsSafeHandle creds);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_start(ServerSafeHandle server);
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_server_request_call(ServerSafeHandle server, CompletionQueueSafeHandle cq, RequestCallContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_cancel_all_calls(ServerSafeHandle server);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_shutdown_and_notify_callback(ServerSafeHandle server, CompletionQueueSafeHandle cq, BatchContextSafeHandle ctx);
            [DllImport(pluginName)]
            public static extern void grpcsharp_server_destroy(IntPtr server);
            [DllImport(pluginName)]
            public static extern AuthContextSafeHandle grpcsharp_call_auth_context(CallSafeHandle call);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_auth_context_peer_identity_property_name(AuthContextSafeHandle authContext);  // returns const char*
            [DllImport(pluginName)]
            public static extern AuthContextSafeHandle.NativeAuthPropertyIterator grpcsharp_auth_context_property_iterator(AuthContextSafeHandle authContext);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_auth_property_iterator_next(ref AuthContextSafeHandle.NativeAuthPropertyIterator iterator);  // returns const auth_property*
            [DllImport(pluginName)]
            public static extern void grpcsharp_auth_context_release(IntPtr authContext);
            [DllImport(pluginName)]
            public static extern Timespec gprsharp_now(ClockType clockType);
            [DllImport(pluginName)]
            public static extern Timespec gprsharp_inf_future(ClockType clockType);
            [DllImport(pluginName)]
            public static extern Timespec gprsharp_inf_past(ClockType clockType);
            [DllImport(pluginName)]
            public static extern Timespec gprsharp_convert_clock_type(Timespec t, ClockType targetClock);
            [DllImport(pluginName)]
            public static extern int gprsharp_sizeof_timespec();
            [DllImport(pluginName)]
            public static extern CallError grpcsharp_test_callback([MarshalAs(UnmanagedType.FunctionPtr)] OpCompletionDelegate callback);
            [DllImport(pluginName)]
            public static extern IntPtr grpcsharp_test_nop(IntPtr ptr);
            [DllImport(pluginName)]
            public static extern void grpcsharp_test_override_method(string methodName, string variant);
        }
#endif
    }
}
